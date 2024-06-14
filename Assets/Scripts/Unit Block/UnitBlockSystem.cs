using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBlockSystem : MonoBehaviour {
    #region private members
    private IGameManager _gameManager;
    private List<UnitBlock> _unitBlocks = new List<UnitBlock>();
    private Vector3 _blockSize;

    [SerializeField]
    private UnitBlock unitBlockPrefab;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

    public UnitBlock CreateUnitBlock(List<Cell> cells, List<IUnit> units, Polyomino polyomino, Cell topLeftCell) {
        UnitBlock unitBlock = Instantiate(unitBlockPrefab);
        unitBlock.Init(cells, units, polyomino);

        _unitBlocks.Add(unitBlock);

        //Set position
        if (_blockSize == Vector3.zero) {
            _blockSize = cells[0].GetComponent<SpriteRenderer>().bounds.size;
        }

        Vector2 topLeftPos = topLeftCell.transform.position;
        int[,] shape = polyomino.Shape;
        int col = shape.GetLength(0);
        int row = shape.GetLength(1);
        Vector2 center = Vector2.zero;

        if (col % 2 == 0) {
            center.x = topLeftPos.x + (col / 2 - 0.5f) * _blockSize.x;
        }
        else {
            center.x = topLeftPos.x + (col / 2) * _blockSize.x;
        }

        if (row % 2 == 0) {
            center.y = topLeftPos.y - (row / 2 - 0.5f) * _blockSize.y;
        }
        else {
            center.y = topLeftPos.y - (row / 2) * _blockSize.y;
        }

        unitBlock.transform.position = center;

        return unitBlock;
    }

    public UnitBlock GetUnitBlock(Cell cell) {
        foreach (UnitBlock block in _unitBlocks) {
            if (block.IsContain(cell))
                return block;
        }

        return null;
    }
}
