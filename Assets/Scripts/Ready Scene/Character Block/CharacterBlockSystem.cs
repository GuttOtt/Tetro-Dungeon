using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class CharacterBlockSystem : MonoBehaviour {
    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();
    [SerializeField] private CharacterBlock _characterBlockPrefab;
    private CharacterBlock _selectedBlock;
    private Vector3 _selectedBlockOriginalPos;

    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private Board _board;

    private bool _isInputOn = true;

    void Update() {
        SelectBlock();
        MoveSelectedBlock();
        UnSelectBlock();
        SpinBlock();
    }

    public CharacterBlock CreateCharacterBlock(CharacterBlockConfig config, int currentLevel) {
        CharacterBlock newBlock = Instantiate(_characterBlockPrefab);
        newBlock.Init(config, _characterBlocks.Count, currentLevel);
        _characterBlocks.Add(newBlock);

        return newBlock;
    }

    public CharacterBlock CreateCharacterBlock(CharacterBlockData data) {
        CharacterBlock newBlock = CreateCharacterBlock(data.Config, data.Level);

        //Spin
        newBlock.Spin(data.SpinDegree);
        
        //Move and Place
        Vector2Int centerCellIndex = data.CenterCellIndex;
        Cell centerCell = _board.GetCell(centerCellIndex.x, centerCellIndex.y);
        Vector3 centerCellPos = centerCell.transform.position;

        BlockPart centerBlockPart = newBlock.CenterBlockPart;
        Vector3 centerBlockPartPos = centerBlockPart.transform.position;

        Vector3 vectorDifference = centerBlockPartPos - centerCellPos;
        newBlock.transform.position -= vectorDifference;

        TryPlace(newBlock);

        return newBlock;
    }

    #region Selection and Dragging Control
    private void SelectBlock() {
        if (!_isInputOn || !Input.GetMouseButton(0) || _selectedBlock != null) {
            return;
        }

        BlockPart selectedBlockPart = Utils.Pick<BlockPart>();
        if (selectedBlockPart == null) return;

        CharacterBlock selectedBlock = selectedBlockPart.CharacterBlock;
        if (selectedBlock == null) return;

        _selectedBlock = selectedBlock;
        _selectedBlockOriginalPos = _selectedBlock.transform.position;

        //가장 위로 가게 하기 위해 SortingLayer 설정
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        selectedBlock.ChangeSortingLayer(draggingLayerID);

        //Unplace
        selectedBlock.Unplace();
    }

    private void MoveSelectedBlock() {
        if (_selectedBlock == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = _selectedBlock.transform.position.z;
        _selectedBlock.transform.position = mousePosition;
    }

    private void UnSelectBlock() {
        if (!Input.GetMouseButtonUp(0) || _selectedBlock == null) return;

        //SortingLayer 원래대로 되돌리기
        int illustLyaerID = SortingLayer.NameToID("Illust");
        _selectedBlock.ChangeSortingLayer(illustLyaerID);

        //Placing
        bool isPlaced = TryPlace();
        if (!isPlaced) {
            //Inventory에 있었던 블럭일 경우
            if (_inventorySystem.ContainsItem(_selectedBlock)) {
                if (!_inventorySystem.IsInsideArea(_selectedBlock)) {
                    _selectedBlock.transform.position = _selectedBlockOriginalPos;
                }
            }
            //Place 되어 있던 상태일 때
            else {
                if (_inventorySystem.IsInsideArea(_selectedBlock)) {
                    _selectedBlock.Unplace();
                    _inventorySystem.Add(_selectedBlock);
                }
                else {
                    _selectedBlock.transform.position = _selectedBlockOriginalPos;
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
        if (block.IsPlacable()) {
            block.Place();
            _inventorySystem.Remove(block);
            return true;
        }
        else {
            return false;
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
