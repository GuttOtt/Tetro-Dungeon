using Array2DEditor;
using Cysharp.Threading.Tasks.Triggers;
using Extensions;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterBlock : MonoBehaviour, IItem {
    private string _name;
    private Sprite _illust;
    private int _level;
    private Array2DBool _shape;
    private CharacterBlockConfig _config;
    private List<Equipment> _equipments = new List<Equipment>();

    [SerializeField] private SpriteRenderer _illustRenderer;

    [SerializeField] private BlockPart _blockPartPrefab;
    [SerializeField] private Transform _blockPartsRoot;
    private List<BlockPart> _blockParts = new List<BlockPart>();

    [SerializeField] private bool _isPlaced = false;
    private int _spinDegree = 0;
    private BlockPart _centerBlockPart;

    public bool IsPlaced { get => _isPlaced; }
    public Vector2Int CenterCellPos {
        get {
            Cell centerCell = _centerBlockPart.PickCell();
            if (centerCell == null)
                return default(Vector2Int);
            else
                return new Vector2Int(centerCell.position.col, centerCell.position.row);
        }
    }
    public BlockPart CenterBlockPart { get => _centerBlockPart; }
    public CharacterBlockConfig Config { get => _config; }
    public List<Equipment> Equipments { get => _equipments; }
    public int SpinDegree { get => _spinDegree; }

    #region Stats
    private int _maxHP, _attack, _spellPower, _defence, _spellDefence, _range, _speed;
    public int MaxHP { get => _maxHP; }
    public int Attack { get => _attack; }
    public int SpellPower { get => _spellPower; }
    public int Defence { get => _defence; }
    public int SpellDefence { get => _spellDefence; }
    public int Speed { get => _speed; }
    public int Range { get => _range; }
    public int OriginalHP { get => Config.MaxHP; }
    public int OriginalAttack { get => Config.Attack; }
    public int OriginalSpellPower { get => Config.SpellPower; }
    public int OriginalDefence { get => Config.Defence; }
    public int OriginalSpellDefence { get => Config.SpellDefence; }
    public int OriginalSpeed { get => Config.Speed; }
    public int OriginalRange { get => Config.Range; }

    #endregion


    public void Init(CharacterBlockConfig config, int id, int currentLvl = 1) {
        _config = config;
        _name = config.name;
        _illust = config.Illust;
        _level = currentLvl;
        _illustRenderer.sprite = _illust;

        //Stats
        _maxHP = config.MaxHP;
        _attack = config.Attack;
        _defence= config.Defence;
        _spellPower = config.SpellPower;
        _spellDefence = config.Defence;
        _speed = config.Speed;
        _range = config.Range;

        CreateBlockParts(config.GetShape(currentLvl), id + 1);
        _illustRenderer.sortingOrder = id + 1;
    }

    private void CreateBlockParts(Array2DBool shape, int sortingOrderFront) {
        _shape = shape;

        int x = shape.GridSize.x;
        int y = shape.GridSize.y;

        Vector2 blockSize = _blockPartPrefab.Size;

        float xOrigin = x % 2 == 0 ? -(x / 2 - 0.5f) * blockSize.x : -(x / 2) * blockSize.x;
        float yOrigin = y % 2 == 0 ? (y / 2 - 0.5f) * blockSize.y : (y / 2) * blockSize.y;

        Vector2Int centerIndex = _config.GetCenterIndex(_level);
        int xCenter = centerIndex.x;
        int yCenter = centerIndex.y;

        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                if (shape.GetCell(i, j) == true) {
                    Vector2 localPosition = new Vector2(xOrigin + i * blockSize.x, yOrigin - j * blockSize.y);
                    Vector2Int location = new Vector2Int(i, j);
                    BlockPart newBlockPart = CreateBlock(localPosition, sortingOrderFront, location);
                    _blockParts.Add(newBlockPart);

                    if (xCenter == i && yCenter == j) {
                        _centerBlockPart = newBlockPart;
                    }
                }
            }
        }

        if (_centerBlockPart == null) {
            Debug.LogError("CharacterBlock�� Center�� �����Ǿ� ���� �ʽ��ϴ�. CharacterBlockConfig�� Ȯ�����ּ���." +
                $"CharacterBlock : {_config.name}");
        }
    }

    private BlockPart CreateBlock(Vector2 localPosition, int frontSortingOrder, Vector2Int location) {
        BlockPart blockPart = Instantiate(_blockPartPrefab, transform);
        blockPart.Init(this, frontSortingOrder, location);
        blockPart.transform.localPosition = localPosition;
        blockPart.transform.parent = _blockPartsRoot;

        return blockPart;
    }

    public void Spin(bool isClockwise) {
        int spinDegree = 0;

        if (!isClockwise) {
            spinDegree = -90;
        }
        else {
            spinDegree = 90;
        }

        transform.Rotate(0, 0, -spinDegree);
        _spinDegree += spinDegree;

    }

    public void Spin(int spinDegree) {
        if (spinDegree < 0) {
            for (int i = 0; i < -spinDegree / 90; i++) {
                Spin(false);
            }
        }
        else {
            for (int i = 0; i < spinDegree / 90; i++) {
                Spin(true);
            }
        }
    }

    public void Place() {
        foreach (BlockPart blockPart in _blockParts) {
            Cell cellUnder = blockPart.PickCell();
            blockPart.Cell = cellUnder;
        }

        //������ �� �ϳ��� ���� Cell���� ����� position ���� ���ϱ�
        Vector3 blockPartPos = _blockParts[0].transform.position;
        Vector3 cellPos = _blockParts[0].Cell.transform.position;
        Vector3 vectorDifference = blockPartPos - cellPos;

        transform.position -= vectorDifference;

        _isPlaced = true;
    }

    public void Place(Cell centerCell) {
        foreach (BlockPart blockPart in _blockParts) {
            Cell cellUnder = blockPart.PickCell();
            blockPart.Cell = cellUnder;
        }

        Vector3 centerCellPos = centerCell.transform.position;
        Vector3 centerBlockPartPos = CenterBlockPart.transform.position;

        Vector3 vectorDifference = centerBlockPartPos - centerCellPos;
        transform.position -= vectorDifference;

        _isPlaced = true;
    }

    public bool IsPlacable() {
        foreach (BlockPart blockPart in _blockParts) {
            Cell cellUnder = blockPart.PickCell();
            BlockPart blockUnder = blockPart.PickBlockPart();
            if (cellUnder == null || blockUnder != null) {
                return false;
            }
        }

        return true;
    }

    public void Unplace() {
        foreach (BlockPart blockPart in _blockParts) {
            Cell cell = blockPart.Cell;
            if (cell != null) {
                blockPart.Cell = null;
            }
        }

        _isPlaced = false;
    }

    public void ChangeSortingLayer(int sortingLayerID) {
        _illustRenderer.sortingLayerID = sortingLayerID;
        
        foreach (BlockPart blockPart in _blockParts) {
            blockPart.SetSortingLayer(sortingLayerID);
        }
    }

    public void ChangeEquipmentSortingLayer(int sortingLayerID) {
        foreach (Equipment equipment in _equipments) {
            equipment.ChangeSortingLayer(sortingLayerID);
        }
    }

    public CharacterBlockData GetData() {
        List<EquipmentData> equipmentDatas = new List<EquipmentData>();

        foreach (Equipment equipment in _equipments) {
            EquipmentData data = equipment.GetData();
            equipmentDatas.Add(data);
        }

        return new CharacterBlockData(_config, _level, CenterCellPos, _spinDegree, equipmentDatas);
    }

    public BlockPart GetBlockPart(int x, int y) {
        foreach (BlockPart blockPart in _blockParts) {
            if (blockPart.Location.x == x && blockPart.Location.y == y) {
                return blockPart;
            }
        }

        return null;
    }

    public void Equip(Equipment equipment) {
        _equipments.Add(equipment);
        equipment.transform.SetParent(transform);

        _maxHP += equipment.Config.MaxHP;
        _attack += equipment.Config.Attack;
        _defence += equipment.Config.Defence;
        _spellPower += equipment.Config.SpellPower;
        _spellDefence += equipment.Config.SpellDefence;
        _speed += equipment.Config.Speed;
        _range += equipment.Config.Range;
    }

    public void Unequip(Equipment equipment) {
        _equipments.Remove(equipment);

        _maxHP -= equipment.Config.MaxHP;
        _attack -= equipment.Config.Attack;
        _defence -= equipment.Config.Defence;
        _spellPower -= equipment.Config.SpellPower;
        _spellDefence -= equipment.Config.Defence;
        _speed -= equipment.Config.Speed;
        _range -= equipment.Config.Range;
    }
}