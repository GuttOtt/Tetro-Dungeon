using Assets.Scripts;
using AYellowpaper.SerializedCollections.Editor.Data;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class CharacterBlockSystem : MonoBehaviour {
    [SerializeField] private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();
    [SerializeField] private CharacterBlock _characterBlockPrefab;
    private CharacterBlock _selectedBlock;
    private Vector3 _selectedBlockOriginalPos;

    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private EquipmentSystem _equipmentSystem;
    [SerializeField] private ShopSystem _shopSystem;
    [SerializeField] private Board _board;
    [SerializeField] private SimpleMonoButton _levelUpButton;

    private bool _isInputOn = true;

    public event Action<CharacterBlock> OnPlace;
    public event Action<CharacterBlock> OnUnplace;


    void Update() {
        SelectBlock();
        MoveSelectedBlock();
        UnSelectBlock();
        SpinBlock();

        //Level up Debugging
        BlockPart selectedBlockPart = Utils.Pick<BlockPart>();
        if (selectedBlockPart == null) return;

        CharacterBlock block = selectedBlockPart.CharacterBlock;
        if (block == null) return;

        if (block != null && Input.GetKeyDown(KeyCode.Space)) {
            LevelUp(block);
        }
    }

    public CharacterBlock LevelUp(CharacterBlock characterBlock) {
        if (_shopSystem.ContainsItem(characterBlock)) {
            return null;
        }
        else if (characterBlock.CurrentLevel == characterBlock.Config.MaxLevel){
            Debug.Log("���̻� �������� �� �����ϴ�.");
            return null;
        }
        else if (Player.Instance.CurrentMoney < characterBlock.LevelUpCost) {
            Debug.Log("�������� �ʿ��� ���� �����մϴ�.");
            return null;
        }

        UnplaceBlock(characterBlock);
        _inventorySystem.Remove(characterBlock);
        
        //Get Data
        CharacterBlockData data = characterBlock.GetData();
        data.Level++;
        
        //Create Level Up Block
        CharacterBlock newBlock = CreateCharacterBlock(data, false);
        _inventorySystem.Add(newBlock);

        //Maintain Position if it's in the inventory
        if (_inventorySystem.IsInsideArea(characterBlock)) {
            newBlock.transform.position = characterBlock.transform.position;
        }

        //Pay cost
        Player.Instance.CurrentMoney -= characterBlock.LevelUpCost;
        _shopSystem.UpdateMoneyText();

        //Delete
        _characterBlocks.Remove(characterBlock);
        Destroy(characterBlock.gameObject);


        return newBlock;
    }
    
    int idCount = 0;
    public CharacterBlock CreateCharacterBlock(CharacterBlockConfig config, int currentLevel) {
        CharacterBlock newBlock = Instantiate(_characterBlockPrefab);
        newBlock.Init(config, idCount, currentLevel);
        idCount++;
        
        _characterBlocks.Add(newBlock);

        return newBlock;
    }

    public CharacterBlock CreateCharacterBlock(CharacterBlockData data, bool isOnBoard = false) {
        CharacterBlock newBlock = CreateCharacterBlock(data.Config, data.Level);

        //Spin
        newBlock.Spin(data.SpinDegree);
        
        //Move and Place
        if (isOnBoard) {
            Vector2Int centerCellIndex = data.CenterCellIndex;
            Cell centerCell = _board.GetCell(centerCellIndex.x, centerCellIndex.y);

            newBlock.Place(centerCell);
            OnPlace?.Invoke(newBlock);
        }

        //Equipments
        List<EquipmentData> equipmentDatas = data.Equipments;
        if (equipmentDatas != null) {
            foreach (EquipmentData equipmentData in equipmentDatas) {
                _equipmentSystem.CreateEquipment(equipmentData, newBlock);
            }
        }

        return newBlock;
    }

    #region Selection and Dragging Control
    public void SetInputOff() {
        _isInputOn = false;
    }

    private void SelectBlock() {
        if (!_isInputOn || !Input.GetMouseButtonDown(0) || _selectedBlock != null) {
            return;
        }

        if (Utils.Pick<BlockPart_Equipment>() != null)
            return;

        BlockPart selectedBlockPart = Utils.Pick<BlockPart>();
        if (selectedBlockPart == null) return;

        CharacterBlock selectedBlock = selectedBlockPart.CharacterBlock;
        if (selectedBlock == null) return;

        _selectedBlock = selectedBlock;
        _selectedBlockOriginalPos = _selectedBlock.transform.position;

        //���� ���� ���� �ϱ� ���� SortingLayer ����
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        selectedBlock.ChangeSortingLayer(draggingLayerID);

        //Equipments���� SortingLayer ����
        int draggingEquipmentLayerID = SortingLayer.NameToID("Dragging Equipment");
        selectedBlock.ChangeEquipmentSortingLayer(draggingEquipmentLayerID);

        //Unplace
        UnplaceBlock(_selectedBlock);
    }

    private void UnplaceBlock(CharacterBlock characterBlock) {
        //Unplace
        if (characterBlock.IsPlaced) {
            OnUnplace?.Invoke(characterBlock);
            foreach(Equipment equipment in characterBlock.Equipments) {
                _equipmentSystem.UnplaceFromBoard(equipment);
            }
        }
        characterBlock.Unplace();
    }

    private void MoveSelectedBlock() {
        if (_selectedBlock == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = _selectedBlock.transform.position.z;
        _selectedBlock.transform.position = mousePosition;
    }

    private void UnSelectBlock() {
        if (!Input.GetMouseButtonUp(0) || _selectedBlock == null) return;

        //SortingLayer ������� �ǵ�����
        int illustLyaerID = SortingLayer.NameToID("Illust");
        _selectedBlock.ChangeSortingLayer(illustLyaerID);

        //Placing
        bool isPlaced = TryPlace();
        if (!isPlaced) {
            //Inventory�� �־��� ���� ���
            if (_inventorySystem.ContainsItem(_selectedBlock)) {
                if (!_inventorySystem.IsInsideArea(_selectedBlock)) {
                    _selectedBlock.transform.position = _selectedBlockOriginalPos;
                }
            }
            //Shop�� �ִ� ���¿��� ���
            else if (_shopSystem.ContainsItem(_selectedBlock)) {
                //Shop -> Inventory
                if (_inventorySystem.IsInsideArea(_selectedBlock)
                    && _shopSystem.IsAffordable(_selectedBlock)) {
                    _shopSystem.Buy(_selectedBlock);
                    _inventorySystem.Add(_selectedBlock);
                }
                else {
                    _selectedBlock.transform.position = _selectedBlockOriginalPos;
                }
            }
            //Place �Ǿ� �ִ� ������ ��
            else {
                if (_inventorySystem.IsInsideArea(_selectedBlock)) {
                    _selectedBlock.Unplace();
                    _inventorySystem.Add(_selectedBlock);
                }
                else {
                    _selectedBlock.transform.position = _selectedBlockOriginalPos;
                    Place(_selectedBlock);
                }
            }
        }

        _selectedBlock = null;
    }

    private void SpinBlock() {
        if (_selectedBlock == null) return;

        bool isClockwise;
        if (Input.GetKeyDown(KeyCode.Q))
            isClockwise = false;
        else if (Input.GetKeyDown(KeyCode.E))
            isClockwise = true;
        else
            return;

        _selectedBlock.Spin(isClockwise);
    }
    #endregion

    private bool TryPlace() {
        if (_selectedBlock == null)
            return false;

        return TryPlace(_selectedBlock);
    }

    private bool TryPlace(CharacterBlock block) {
        if (!block.IsPlacable()) {
            return false;
        }

        if (_shopSystem.ContainsItem(block)) {
            if (!_shopSystem.IsAffordable(block)) {
                Debug.Log("���� �����մϴ�.");
                return false;
            }
            _shopSystem.Buy(block);
        }

        Place(block);

        return true;
    }

    private void Place(CharacterBlock block)
    {
        block.Place();
        _inventorySystem.Remove(block);
        OnPlace?.Invoke(block);

        foreach(Equipment equipment in block.Equipments) {
            _equipmentSystem.PlaceOnBoard(equipment);
        }

    }

  public List<CharacterBlockData> GetCharacterBlockDatasOnBoard() {
        List<CharacterBlockData> datas = new List<CharacterBlockData>();

        foreach (CharacterBlock block in _characterBlocks) {
            if (block.IsPlaced) {
                datas.Add(block.GetData());
            }
        }

        return datas;
    }
}
