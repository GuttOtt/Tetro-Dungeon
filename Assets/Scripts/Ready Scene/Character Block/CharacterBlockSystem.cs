using Array2DEditor;
using Assets.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [SerializeField] private SimpleMonoButton levelUpButton;
    [SerializeField] private BlockPartMarker blockPartMarkerPrefab;
    [SerializeField] private CharacterBlockInfoSystem characterBlockInfoSystem;


    private bool _isInputOn = true;

    public event Action<CharacterBlock> OnPlace;
    public event Action<CharacterBlock> OnUnplace;


    void Start()
    {
        if (levelUpButton != null)
        {
            levelUpButton.onClick += HandleLevelUpButton;
        }
    }

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
    }

    #region LevelUp
    private void HandleLevelUpButton()
    {
        StartLevelUp().Forget();
    }

    private async UniTask StartLevelUp()
    {
        CharacterBlock currentBlock = characterBlockInfoSystem.CurrentCharacterBlock;
        characterBlockInfoSystem.ClosePanel();
        await LevelUp(currentBlock);
    } 

    public async UniTask<bool> LevelUp(CharacterBlock characterBlock)
    {
        if (!characterBlock.IsPlaced)
        {
            Debug.Log("���� ��ġ�Ǿ� ���� �ʽ��ϴ�.");
            return false;
        }

        if (_shopSystem.ContainsItem(characterBlock))
        {
            return false;
        }
        else if (characterBlock.CurrentLevel == characterBlock.Config.MaxLevel)
        {
            Debug.Log("���̻� �������� �� �����ϴ�.");
            return false;
        }
        else if (Player.Instance.CurrentMoney < characterBlock.LevelUpCost)
        {
            Debug.Log("�������� �ʿ��� ���� �����մϴ�.");
            return false;
        }

        await AddNewBlockPartAsync(characterBlock);

        //Gain Level and Stat
        characterBlock.GainLevel();

        //Pay cost
        Player.Instance.CurrentMoney -= characterBlock.LevelUpCost;
        _shopSystem.UpdateMoneyText();

        return true;
    }

    private async UniTask AddNewBlockPartAsync(CharacterBlock characterBlock)
    {
        // 1. Board ������ �ø� �� �ִ� ������ �˻�
        bool[,] shape = characterBlock.CurrentShape;

        Dictionary<Cell, Vector2Int> cellLocationMap = new Dictionary<Cell, Vector2Int>(); //Cell�� �ش� Cell�� shape ���� location�� ����

        foreach (BlockPart blockPart in characterBlock.BlockParts)
        {
            AddAdjacentCellIfValid(cellLocationMap, blockPart, shape, new Vector2Int(0, -1));
            AddAdjacentCellIfValid(cellLocationMap, blockPart, shape, new Vector2Int(0, 1));
            AddAdjacentCellIfValid(cellLocationMap, blockPart, shape, new Vector2Int(-1, 0));
            AddAdjacentCellIfValid(cellLocationMap, blockPart, shape, new Vector2Int(1, 0));
        }

        // 2. �ش� �����鿡 BlockPart ��Ŀ�� ����
        List<BlockPartMarker> blockPartMarkers = new List<BlockPartMarker>();
        foreach (Cell cell in cellLocationMap.Keys)
        {
            BlockPartMarker marker = Instantiate(blockPartMarkerPrefab, cell.transform.position, Quaternion.identity);
            marker.cell = cell;
            marker.location = cellLocationMap[cell];
            marker.transform.SetParent(cell.transform);
            blockPartMarkers.Add(marker);
        }

        // 3. �÷��̾ BlockPart ��Ŀ�� Ŭ���� ������ ���

        BlockPartMarker clickedMarker = null;
        while (clickedMarker == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickedMarker = Utils.Pick<BlockPartMarker>();
            }
            await UniTask.Yield();
        }

        // 4. Ŭ���� BlockPart ��Ŀ�� ���� ���ο� BlockPart�� ����
        Vector2Int clickedLocation = clickedMarker.location;
        Vector2 localPosition = clickedMarker.transform.position - characterBlock.transform.position;
        localPosition = Quaternion.AngleAxis(characterBlock.SpinDegree, Vector3.forward) * localPosition;
        BlockPart newBlockPart = characterBlock.AddBlockPart(localPosition, clickedLocation);

        // 5. BlockPart ��Ŀ ����
        foreach (BlockPartMarker marker in blockPartMarkers)
        {
            Destroy(marker.gameObject);
        }
    }

    private bool IsValidInShape(BlockPart blockPart, bool[,] shape, Vector2Int offset)
    {
        
        Vector2Int centerLocation = blockPart.Location;
        Vector2Int newLocation = new Vector2Int(centerLocation.x + offset.x, centerLocation.y + offset.y);

        Debug.Log($"Validating in shape. newLocation: {newLocation}");

        if (newLocation.x < 0 || newLocation.x >= shape.GetLength(0) ||
            newLocation.y < 0 || newLocation.y >= shape.GetLength(1) ||
            shape[newLocation.x, newLocation.y] == true)
        {
            return false;
        }

        return true;
    }

    private Cell GetValidCell(BlockPart blockPart, bool[,] shape, Vector2Int offset)
    {
        if (!IsValidInShape(blockPart, shape, new Vector2Int(offset.x, offset.y)))
            return null;

        Vector2Int centerCellPos = new Vector2Int(blockPart.Cell.position.col, blockPart.Cell.position.row);
        Vector2Int newCellPos = new Vector2Int(centerCellPos.x + offset.x, centerCellPos.y + offset.y);

        Cell newCell = _board.GetCell(newCellPos.x, newCellPos.y);

        return newCell;
    }

    private void AddAdjacentCellIfValid(Dictionary<Cell, Vector2Int> cellLocationMap, BlockPart blockPart, bool[,] shape, Vector2Int offset)
    {
        Cell newCell = GetValidCell(blockPart, shape, offset);
        if (newCell != null && !cellLocationMap.ContainsKey(newCell))
            cellLocationMap.Add(newCell, new Vector2Int(blockPart.Location.x + offset.x, blockPart.Location.y + offset.y));
    }
    #endregion


    int idCount = 0;
    public CharacterBlock CreateCharacterBlock(CharacterBlockConfig config, int currentLevel) {
        CharacterBlock newBlock = Instantiate(_characterBlockPrefab);
        newBlock.Init(config, idCount, currentLevel);
        idCount++;
        
        _characterBlocks.Add(newBlock);

        return newBlock;
    }

    public async UniTask<CharacterBlock> CreateCharacterBlock(CharacterBlockData data, bool isOnBoard = false) {
        CharacterBlock newBlock = CreateCharacterBlock(data.Config, data.Level);
        
        //Shape
        bool[,] originalShape = newBlock.Config.GetShape(1).GetCells();
        bool[,] currentShape = data.Shape;

        for (int x = 0; x < currentShape.GetLength(0); x++)
        {
            for (int y = 0; y < currentShape.GetLength(1); y++)
            {
                if (currentShape[x, y] == true && originalShape[x, y] == false)
                {
                    newBlock.AddBlockPart(new Vector2Int(x, y));
                    Debug.Log("AddBlockPart");
                }
            }    
        }

        //Spin
        newBlock.Spin(data.SpinDegree);

        //Move and Place
        if (isOnBoard)
        {
            Vector2Int centerCellIndex = data.CenterCellIndex;
            Cell centerCell = _board.GetCell(centerCellIndex.x, centerCellIndex.y);

            await newBlock.Place(centerCell);
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
    public void SetInputOn() {
        _isInputOn = true;
    }

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
            // �Ǹ� ó��
            if (_shopSystem.IsInsideShopArea(_selectedBlock) && !_shopSystem.ContainsItem(_selectedBlock))
            {
                _shopSystem.Sell(_selectedBlock);
                _inventorySystem.Remove(_selectedBlock);

                // characterBlock�� ������ Equipment���� �Ǹ�
                foreach (Equipment equipment in _selectedBlock.Equipments)
                {
                    _shopSystem.Sell(equipment);
                    _equipmentSystem.DestroyEquipment(equipment);
                }

                Debug.Log("Sold characterBlock: " + _selectedBlock.name);

                _characterBlocks.Remove(_selectedBlock);
                Destroy(_selectedBlock.gameObject);
            }
            //Shop�� �ִ� ���¿��� ���
            else if (_shopSystem.ContainsItem(_selectedBlock))
            {
                _selectedBlock.transform.position = _selectedBlockOriginalPos;
            }
            //Place �Ǿ� �ִ� ������ ��
            else
            {
                _selectedBlock.transform.position = _selectedBlockOriginalPos;
                Place(_selectedBlock);
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
