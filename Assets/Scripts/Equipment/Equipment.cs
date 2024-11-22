using Array2DEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
    private EquipmentConfig _config;
    private Array2DBool _shape;
    private List<BlockPart_Equipment> _blockParts = new List<BlockPart_Equipment>();
    private int _spinDegree;
    private bool _isPlaced;
    private Vector2Int _locationInCharacter;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private BlockPart_Equipment _blockPartPrefab;
    [SerializeField] private Transform _blockPartsRoot;

    public BlockPart_Equipment CenterBlockPart { get => _blockParts[0]; }

    #region Init
    public void Init(EquipmentConfig config) {
        _config = config;
        _spriteRenderer.sprite = config.Sprite;

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
            _spinDegree += 90;
        }
        else {
            transform.Rotate(0, 0, -90);
            _spinDegree -= 90;
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
        Vector3 blockPartPos = centerBlock.transform.position;
        Vector3 characterBlockPartPos = centerCharacterBlock.transform.position;
        Vector3 vectorDifference = blockPartPos - characterBlockPartPos;

        transform.position -= vectorDifference;

        _isPlaced = true;

        _locationInCharacter = centerCharacterBlock.Location;
        centerCharacterBlock.CharacterBlock.Equip(this);

        //반투명
        _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

    public void Unplace() {
        _isPlaced = false;
        transform.parent = null;

        //불투명
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void ChangeSortingLayer(int sortingLayerID) {
        _spriteRenderer.sortingLayerID = sortingLayerID;

        foreach (BlockPart_Equipment blockPart in _blockParts) {
            //blockPart.SetSortingLayer(sortingLayerID);
        }
    }

    public EquipmentData GetData() {
        EquipmentData data = new EquipmentData();

        data.Config = _config;
        data.Location = _locationInCharacter;
        data.SpinDegree = _spinDegree;

        return data;
    }
}
