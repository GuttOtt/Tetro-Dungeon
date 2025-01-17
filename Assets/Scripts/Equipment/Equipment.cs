using Array2DEditor;
using AYellowpaper.SerializedCollections;
using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipment : MonoBehaviour, IItem {
    private EquipmentConfig _config;
    private Array2DBool _shape;
    private List<BlockPart_Equipment> _blockParts = new List<BlockPart_Equipment>();
    [SerializeField] private int _spinDegree;
    private bool _isPlaced;
    private CharacterBlock _characterBlock;
    private Vector2Int _locationInCharacter;
    private Sprite _sprite;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private BlockPart_Equipment _blockPartPrefab;
    [SerializeField] private Transform _blockPartsRoot;

    public BlockPart_Equipment CenterBlockPart { get => _blockParts[0]; }
    public int SpinDegree { get => _spinDegree; set => _spinDegree = value; }
    public EquipmentConfig Config { get => _config; }
    public Stat Stat { get => Config.Stat; }
    public SerializedDictionary<SynergyTypes, int> SynergyDict { get => Config.SynergyDict; }
    public Sprite Sprite { get => _sprite; }

    #region Init
    public void Init(EquipmentConfig config) {
        _config = config;
        _spriteRenderer.sprite = config.Sprite;
        _sprite = config.Sprite;

        CreateBlockParts(config.Shape);
    }

    private void CreateBlockParts(Array2DBool shape) {
        _shape = _config.Shape;

        int x = shape.GridSize.x;
        int y = shape.GridSize.y;

        Vector2 blockSize = _blockPartPrefab.Size;

        //Top Left
        float xOrigin = x % 2 == 0 ? -(x / 2 - 0.5f) * blockSize.x : -(x / 2) * blockSize.x;
        float yOrigin = y % 2 == 0 ? (y / 2 - 0.5f) * blockSize.y : (y / 2) * blockSize.y;

        
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                if (shape.GetCell(i, j) == true) {
                    Vector2 localPosition = new Vector2(xOrigin + i * blockSize.x, yOrigin - j * blockSize.y);
                    BlockPart_Equipment newBlockPart = CreateBlock(localPosition);
                    _blockParts.Add(newBlockPart);
                }
            }
        }

        SetMarkersOn(false);
    }

    private BlockPart_Equipment CreateBlock(Vector2 localPosition) {
        BlockPart_Equipment blockPart = Instantiate(_blockPartPrefab, transform);
        blockPart.Init(this);
        blockPart.transform.localPosition = localPosition;
        blockPart.transform.parent = _blockPartsRoot;

        return blockPart;
    }

    #endregion


    public void Spin(bool isClockwise) {
        if (!isClockwise) {
            transform.Rotate(0, 0, 90);
            _spinDegree += -90;
        }
        else {
            transform.Rotate(0, 0, -90);
            _spinDegree += 90;
        }
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

    public bool IsPlacable() {
        CharacterBlock character = null;

        foreach (BlockPart_Equipment blockPart in _blockParts) {
            BlockPart characterBlockPart = blockPart.PickBlockPart();
            if (characterBlockPart == null) {
                return false;
            }

            if (character == null) {
                character = characterBlockPart.CharacterBlock;
            }
            else if (character != characterBlockPart.CharacterBlock) {
                return false;
            }
        }

        return true;
    }

    public void Place() {
        //임의의 셀 하나를 택해 Cell과의 상대적 position 차이 구하기
        BlockPart_Equipment centerBlock = _blockParts[0];
        BlockPart centerCharacterBlock = _blockParts[0].PickBlockPart();

        CharacterBlock characterBlock = centerCharacterBlock.CharacterBlock;
        Vector2Int location = centerCharacterBlock.Location;

        Place(characterBlock, location);
    }

    public void Place(CharacterBlock characterBlock, Vector2Int location) {
        if (characterBlock == null) return;

        BlockPart blockPart = characterBlock.GetBlockPart(location.x, location.y);
        Vector3 blockPartPos = blockPart.transform.position;

        BlockPart_Equipment center = CenterBlockPart;
        Vector3 centerPos = center.transform.position;

        Vector3 vectorDifference = centerPos - blockPartPos;
        transform.position -= vectorDifference;

        _locationInCharacter = location;
        blockPart.CharacterBlock.Equip(this);

        //반투명
        _spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        _characterBlock = characterBlock;
        _isPlaced = true;
    }

    public void Unplace() {
        _characterBlock?.Unequip(this);
        _characterBlock = null;

        _isPlaced = false;
        transform.parent = null;

        //불투명
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void ChangeSortingLayer(int sortingLayerID) {
        _spriteRenderer.sortingLayerID = sortingLayerID;
    }

    public EquipmentData GetData() {
        EquipmentData data = new EquipmentData();

        data.Config = _config;
        data.Location = _locationInCharacter;
        data.SpinDegree = _spinDegree;

        return data;
    }

    public void SetMarkersOn(bool isOn) {
        foreach (BlockPart_Equipment blockPart in _blockParts) {
            blockPart.SetMarkerOn(isOn);
        }
    }
}
