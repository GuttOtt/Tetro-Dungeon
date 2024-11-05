using Array2DEditor;
using Extensions;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlock : MonoBehaviour {
    private string _name;
    private Sprite _illust;
    private Array2DBool _shape;
    [SerializeField] private CharacterBlockConfig _config;
    private UnitConfig _unitConfig;

    [SerializeField] private SpriteRenderer _illustRenderer;

    [SerializeField] private BlockPart _blockPartPrefab;
    private List<BlockPart> _blockParts = new List<BlockPart>();

    private void Start() {
    }

    public void Init(CharacterBlockConfig config, int id, int currentLvl = 1) {
        _config = config;
        _name = config.name;
        _unitConfig = config.UnitConfig;
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
        blockPart.transform.localPosition = localPosition;
        blockPart.SetSortingOrder(frontSortingOrder);

        return blockPart;
    }

    public void Spin(bool isClockwise) {

    }

    public void Place() {

    }
}