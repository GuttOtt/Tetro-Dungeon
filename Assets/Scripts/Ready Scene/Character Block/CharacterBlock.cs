using Array2DEditor;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using EnumTypes;
using Extensions;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterBlock : MonoBehaviour, IItem
{
    //Basic Infos
    private string _name;
    private Sprite _illust;
    [SerializeField] private int _level;
    private int id;
    private bool[,] currentShape;
    private CharacterBlockConfig _config;

    //Equipment
    private List<Equipment> _equipments = new List<Equipment>();
    private List<UnitSkill> equipmentSkills = new List<UnitSkill>();

    //Skill
    private UnitSkill _defaultSkill;
    private List<UnitSkill> _passiveSkill = new List<UnitSkill>();
    private List<UnitSkill> _activeSkill = new List<UnitSkill>();
    public UnitSkill DefaultSkill { get => _defaultSkill; }
    public List<UnitSkill> PassiveSkills { get => _passiveSkill; }
    public List<UnitSkill> ActiveSkills { get => _activeSkill; }
    public List<UnitSkill> EquipmentSkills => equipmentSkills;
    public List<UnitSkill> Skills
    {
        get
        {
            List<UnitSkill> allSkills = new List<UnitSkill>();
            allSkills.AddRange(_activeSkill);
            allSkills.AddRange(_passiveSkill);
            return allSkills;
        }
    }
    public int LevelUpCost { get => CurrentLevel + 4; }
    public SerializedDictionary<SynergyTypes, int> SynergyDict { get => _config.GetSynergyDict(_level); }


    //Serialized Fields
    [SerializeField] private SpriteRenderer _illustRenderer;
    [SerializeField] private BlockPart _blockPartPrefab;
    [SerializeField] private Transform _blockPartsRoot;

    //Placing and spining
    private List<BlockPart> _blockParts = new List<BlockPart>();
    [SerializeField] private bool _isPlaced = false;
    [SerializeField] private int _spinDegree = 0;
    private BlockPart _centerBlockPart;


    //Awakenings
    private List<Awakening> _awakenings;
    public List<Awakening> Awakenings { get => _awakenings; }
    private Dictionary<Awakening, bool> _awakeningActivation = new Dictionary<Awakening, bool>();
    public Dictionary<Awakening, bool> AwakeningActivation { get => _awakeningActivation; }

    public bool IsPlaced { get => _isPlaced; }
    public Vector2Int CenterCellPos
    {
        get
        {
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
    public List<BlockPart> BlockParts { get => _blockParts; }
    public int ID => id;

    #region Stats
    private Stat _stat;

    public Stat Stat { get => _stat.DeepCopy(); }
    public Stat OriginalStat { get => Config.Stat; }
    public int CurrentLevel { get => _level; }
    public List<Cell> Cells { get => _blockParts.Select(blockPart => blockPart.Cell).ToList(); }

    public bool[,] CurrentShape { get => currentShape; }
    #endregion


    public void Init(CharacterBlockConfig config, int id, int currentLvl = 1)
    {   
        _config = config;
        _name = config.name;
        _illust = config.Illust;
        _level = currentLvl;
        _illustRenderer.sprite = _illust;
        this.id = id;

        //Stats
        _stat = config.Stat;
        _stat += config.StatForLevelUp * (currentLvl - 1);

        //Skills
        _defaultSkill = SkillFactory.CreateSkill(config.DefaultSkill);
        _passiveSkill = SkillFactory.CreateSkills(config.PassiveSkills);
        _activeSkill = SkillFactory.CreateSkills(config.ActiveSkills);

        //Awakenings
        _awakenings = config.Awakenings.ToList();
        foreach (Awakening awakening in _awakenings)
        {
            _awakeningActivation.Add(awakening, false);
        }

        CreateBlockParts(config.GetShape(1), id + 1);
        _illustRenderer.sortingOrder = id + 1;
    }

    public void AddActiveSkill(UnitSkill skill)
    {
        _activeSkill.Add(skill);
    }

    public void AddPassiveSkill(UnitSkill skill)
    {
        _passiveSkill.Add(skill);
    }

    public void RemovePassiveSkill(UnitSkill skill)
    {
        _passiveSkill.Remove(skill);
    }

    public void RemoveActiveSkill(UnitSkill skill)
    {
        _activeSkill.Remove(skill);
    }

    private void CreateBlockParts(Array2DBool shape, int sortingOrderFront)
    {
        currentShape = shape.GetCells();

        int x = shape.GridSize.x;
        int y = shape.GridSize.y;

        Vector2 blockSize = _blockPartPrefab.Size;

        float xOrigin = x % 2 == 0 ? -(x / 2 - 0.5f) * blockSize.x : -(x / 2) * blockSize.x;
        float yOrigin = y % 2 == 0 ? (y / 2 - 0.5f) * blockSize.y : (y / 2) * blockSize.y;

        Vector2Int centerIndex = _config.GetCenterIndex(_level);
        int xCenter = centerIndex.x;
        int yCenter = centerIndex.y;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (shape.GetCell(i, j) == true)
                {
                    Vector2 localPosition = new Vector2(xOrigin + i * blockSize.x, yOrigin - j * blockSize.y);
                    Vector2Int location = new Vector2Int(i, j);
                    BlockPart newBlockPart = AddBlockPart(localPosition, location);

                    if (xCenter == i && yCenter == j)
                    {
                        _centerBlockPart = newBlockPart;
                    }
                }
            }
        }

        if (_centerBlockPart == null)
        {
            Debug.LogError("CharacterBlock의 Center가 지정되어 있지 않습니다. CharacterBlockConfig를 확인해주세요." +
                $"CharacterBlock : {_config.name}");
        }
    }

    public BlockPart AddBlockPart(Vector2 localPosition, Vector2Int location)
    {
        BlockPart blockPart = Instantiate(_blockPartPrefab, transform);
        blockPart.Init(this, id + 1, location);
        blockPart.transform.localPosition = localPosition;
        blockPart.transform.parent = _blockPartsRoot;

        currentShape[location.x, location.y] = true;
        _blockParts.Add(blockPart);

        return blockPart;
    }

    public BlockPart AddBlockPart(Vector2Int location)
    {
        int x = currentShape.GetLength(0);
        int y = currentShape.GetLength(1);

        Vector2 blockSize = _blockPartPrefab.Size;

        float xOrigin = x % 2 == 0 ? -(x / 2 - 0.5f) * blockSize.x : -(x / 2) * blockSize.x;
        float yOrigin = y % 2 == 0 ? (y / 2 - 0.5f) * blockSize.y : (y / 2) * blockSize.y;

        Vector2 localPosition = new Vector2(xOrigin + location.x * blockSize.x, yOrigin - location.y * blockSize.y);

        return AddBlockPart(localPosition, location);
    }

    public void Spin(bool isClockwise)
    {
        int spinDegree = 0;

        if (!isClockwise)
        {
            spinDegree = -90;
            //Shape와 BlockPart들의 Location 변경
            int originalCol = currentShape.GetLength(0);
            int originalRow = currentShape.GetLength(1);
            foreach (BlockPart blockPart in _blockParts)
            {
                Vector2Int newLocation = new Vector2Int(blockPart.Location.y, originalCol - blockPart.Location.x - 1);
                blockPart.SetLocation(newLocation);
            }
            currentShape = Utils.RotateLeft<bool>(currentShape);
        }
        else
        {
            spinDegree = 90;
            //Shape와 BlockPart들의 Location 변경
            int originalCol = currentShape.GetLength(0);
            int originalRow = currentShape.GetLength(1);
            foreach (BlockPart blockPart in _blockParts)
            {
                Vector2Int newLocation = new Vector2Int(originalRow - 1 - blockPart.Location.y, blockPart.Location.x);
                blockPart.SetLocation(newLocation);
            }
            currentShape = Utils.RotateRight<bool>(currentShape);
        }

        transform.Rotate(0, 0, -spinDegree);
        _spinDegree += spinDegree;

        foreach (Equipment equipment in _equipments)
        {
            equipment.SpinDegree += spinDegree;
        }
    }

    public void Spin(int spinDegree)
    {
        if (spinDegree < 0)
        {
            for (int i = 0; i < -spinDegree / 90; i++)
            {
                Spin(false);
            }
        }
        else if (0 < spinDegree)
        {
            for (int i = 0; i < spinDegree / 90; i++)
            {
                Spin(true);
            }
        }
    }

    public void Place()
    {
        foreach (BlockPart blockPart in _blockParts)
        {
            Cell cellUnder = blockPart.PickCell();
            blockPart.Cell = cellUnder;
        }

        //임의의 셀 하나를 택해 Cell과의 상대적 position 차이 구하기
        Vector3 blockPartPos = _blockParts[0].transform.position;
        Vector3 cellPos = _blockParts[0].Cell.transform.position;
        Vector3 vectorDifference = blockPartPos - cellPos;

        transform.position -= vectorDifference;

        _isPlaced = true;
    }

    public async UniTask Place(Cell centerCell)
    {
        // Force physics update to ensure block part positions are updated
        await UniTask.NextFrame();

        Vector3 centerCellPos = centerCell.transform.position;
        Vector3 centerBlockPartPos = CenterBlockPart.transform.position;

        Vector3 vectorDifference = centerBlockPartPos - centerCellPos;
        transform.position -= vectorDifference;

        await UniTask.NextFrame();

        foreach (BlockPart blockPart in _blockParts)
        {
            Cell cellUnder = blockPart.PickCell();
            blockPart.Cell = cellUnder;
        }

        _isPlaced = true;
    }

    public bool IsPlacable()
    {
        foreach (BlockPart blockPart in _blockParts)
        {
            Cell cellUnder = blockPart.PickCell();
            BlockPart blockUnder = blockPart.PickBlockPart();
            if (cellUnder == null || blockUnder != null)
            {
                return false;
            }
        }

        return true;
    }

    public void Unplace()
    {
        foreach (BlockPart blockPart in _blockParts)
        {
            Cell cell = blockPart.Cell;
            if (cell != null)
            {
                blockPart.Cell = null;
            }
        }

        _isPlaced = false;
    }

    public void ChangeSortingLayer(int sortingLayerID)
    {
        _illustRenderer.sortingLayerID = sortingLayerID;

        foreach (BlockPart blockPart in _blockParts)
        {
            blockPart.SetSortingLayer(sortingLayerID);
        }
    }

    public void ChangeEquipmentSortingLayer(int sortingLayerID)
    {
        foreach (Equipment equipment in _equipments)
        {
            equipment.ChangeSortingLayer(sortingLayerID);
        }
    }

    public CharacterBlockData GetData()
    {
        // Equipment
        List<EquipmentData> equipmentDatas = new List<EquipmentData>();

        foreach (Equipment equipment in _equipments)
        {
            EquipmentData data = equipment.GetData();
            equipmentDatas.Add(data);
        }

        // Shape
        bool[,] shape = currentShape;
        int spinAmount = _spinDegree % 360 / 90;
        bool isClockwise = 0 <= _spinDegree;
        Debug.Log($"spinAmount: {spinAmount}, isClockwise: {isClockwise}");
        for (int i = 0; i < spinAmount; i++)
        {
            if (isClockwise)
                shape = Utils.RotateRight<bool>(shape);

            else
                shape = Utils.RotateLeft<bool>(shape);
        }

        return new CharacterBlockData(_config, _level, CenterCellPos, _spinDegree, shape, equipmentDatas);
    }

    public BlockPart GetBlockPart(int x, int y)
    {
        foreach (BlockPart blockPart in _blockParts)
        {
            if (blockPart.Location.x == x && blockPart.Location.y == y)
            {
                return blockPart;
            }
        }

        return null;
    }

    public void Equip(Equipment equipment)
    {
        _equipments.Add(equipment);
        equipment.transform.SetParent(transform);

        _stat += equipment.Stat;

        foreach (UnitSkill skill in equipment.Skills)
        {
            equipmentSkills.Add(skill);
        }

        UpdateAwakening();
    }

    public void Unequip(Equipment equipment)
    {
        _equipments.Remove(equipment);

        _stat -= equipment.Stat;

        foreach (UnitSkill skill in equipment.Skills)
        {
            equipmentSkills.Remove(skill);
        }

        UpdateAwakening();
    }

    private void UpdateAwakening()
    {
        foreach (Awakening awakening in _awakenings)
        {
            bool isActivated = awakening.UpdateActivation(this);
            _awakeningActivation[awakening] = isActivated;
        }
    }

    public void SetShape(bool[,] shape)
    {
        currentShape = shape;
    }

    public void GainLevel()
    {
        _level++;
        _stat += Config.StatForLevelUp;    
    }
}
