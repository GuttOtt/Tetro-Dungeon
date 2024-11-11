using Array2DEditor;
using Extensions;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlock : MonoBehaviour {
    private string _name;
    private Sprite _illust;
    private Array2DBool _shape;
    private CharacterBlockConfig _config;

    [SerializeField] private SpriteRenderer _illustRenderer;

    [SerializeField] private BlockPart _blockPartPrefab;
    [SerializeField] private Transform _blockPartsRoot;
    private List<BlockPart> _blockParts = new List<BlockPart>();

    private void Start() {
    }

    public void Init(CharacterBlockConfig config, int id, int currentLvl = 1) {
        _config = config;
        _name = config.name;
        _illust = config.Illust;

        _illustRenderer.sprite = _illust;

        CreateBlocks(config.GetShape(currentLvl), id + 1);
        _illustRenderer.sortingOrder = id + 1;
    }

    private void CreateBlocks(Array2DBool shape, int sortingOrderFront) {
        _shape = shape;

        int x = shape.GridSize.x;
        int y = shape.GridSize.y;

        Vector2 blockSize = _blockPartPrefab.Size;

        float xOrigin = x / 2 == 0 ? -(x / 2 - 0.5f) * blockSize.x : -(x / 2) * blockSize.x;
        float yOrigin = y / 2 == 0 ? (y / 2 - 0.5f) * blockSize.y : (y / 2) * blockSize.y;
        
        for (int i = 0; i < x; i++) {
            for (int j = 0; j < y; j++) {
                if (shape.GetCell(i, j) == true) {
                    Vector2 localPosition = new Vector2(xOrigin + i * blockSize.x, yOrigin - j * blockSize.y);
                    _blockParts.Add(CreateBlock(localPosition, sortingOrderFront));
                }
            }
        }
    }

    private BlockPart CreateBlock(Vector2 localPosition, int frontSortingOrder) {
        BlockPart blockPart = Instantiate(_blockPartPrefab, transform);
        blockPart.Init(this, frontSortingOrder);
        blockPart.transform.localPosition = localPosition;
        blockPart.transform.parent = _blockPartsRoot;

        return blockPart;
    }

    public void Spin(bool isClockwise) {
        if (!isClockwise) {
            transform.Rotate(0, 0, 90);
        }
        else {
            transform.Rotate(0, 0, -90);
        }
    }

    public void Place() {
        foreach (BlockPart blockPart in _blockParts) {
            Cell cellUnder = blockPart.PickCell();
            blockPart.Cell = cellUnder;
        }

        //임의의 셀 하나를 택해 Cell과의 상대적 position 차이 구하기
        Vector3 blockPartPos = _blockParts[0].transform.position;
        Vector3 cellPos = _blockParts[0].Cell.transform.position;
        Vector3 vectorDifference = blockPartPos - cellPos;

        transform.position -= vectorDifference;
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
    }

    public void ChangeSortingLayer(int sortingLayerID) {
        _illustRenderer.sortingLayerID = sortingLayerID;
        
        foreach (BlockPart blockPart in _blockParts) {
            blockPart.SetSortingLayer(sortingLayerID);
        }
    }
}