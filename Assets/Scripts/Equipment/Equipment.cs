using Array2DEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour {
    private EquipmentConfig _config;
    private Array2DBool _shape;
    private List<BlockPart> _blockParts = new List<BlockPart>();
    private int _spinDegree;
    private bool _isPlaced;
    private Vector2Int _locationInCharacter;

    [SerializeField] private BlockPart _blockPartPrefab;
    [SerializeField] private Transform _blockPartsRoot;

    public BlockPart CenterBlockPart { get => _blockParts[0]; }

    #region Init
    public void Init(EquipmentConfig config) {
        _config = config;

        CreateBlockParts(config.Shape);
    }

    private void CreateBlockParts(Array2DBool shape) {
        _shape = _config.Shape;

        int x = shape.GridSize.x;
        int y = shape.GridSize.y;

        Vector2 blockSize = _blockPartPrefab.Size;

        float xOrigin = x / 2 == 0 ? -(x / 2 - 0.5f) * blockSize.x : -(x / 2) * blockSize.x;
        float yOrigin = y / 2 == 0 ? (y / 2 - 0.5f) * blockSize.y : (y / 2) * blockSize.y;


        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                if (shape.GetCell(i, j) == true) {
                    Vector2 localPosition = new Vector2(xOrigin + i * blockSize.x, yOrigin - j * blockSize.y);
                    BlockPart newBlockPart = CreateBlock(localPosition);
                    _blockParts.Add(newBlockPart);
                }
            }
        }

    }

    private BlockPart CreateBlock(Vector2 localPosition) {
        BlockPart blockPart = Instantiate(_blockPartPrefab, transform);
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

        foreach (BlockPart blockPart in _blockParts) {
            BlockPart blockPartUnder = blockPart.PickBlockPart();
            if (blockPartUnder == null) {
                return false;
            }

            if (character == null) {
                character = blockPartUnder.CharacterBlock;
            }
            else if (character != blockPartUnder.CharacterBlock) {
                return false;
            }
        }

        return true;
    }

    public void Place() {
        //임의의 셀 하나를 택해 Cell과의 상대적 position 차이 구하기
        Vector3 blockPartPos = _blockParts[0].transform.position;
        Vector3 characterBlockPartPos = _blockParts[0].PickBlockPart().transform.position;
        Vector3 vectorDifference = blockPartPos - characterBlockPartPos;

        transform.position -= vectorDifference;

        _isPlaced = true;

        _blockParts[0].PickBlockPart().CharacterBlock.Equip(this);
    }
}
