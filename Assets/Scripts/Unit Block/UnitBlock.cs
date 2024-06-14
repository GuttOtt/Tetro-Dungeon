using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitBlock : MonoBehaviour {
    private class Position {
        public int col, row;

        public Position(int col, int row) {
            this.col = col;
            this.row = row;
        }
    }

    #region private members
    private List<Cell> _cells;
    private List<IUnit> _units;
    private List<Position> _positions;
    private Polyomino _polyomino;

    [SerializeField] private PolyominoDrawer _drawer;
    #endregion

    public void Init(List<Cell> cells, List<IUnit> units, Polyomino polyomino) {
        _cells = cells.ToList();
        _units = units.ToList();
        _polyomino = polyomino;

        _positions = new List<Position>();
        foreach (Cell cell in _cells) {
            Position pos = new Position(cell.position.col, cell.position.row);
            _positions.Add(pos);
        }

        _drawer?.Draw(_polyomino.Shape);
        SetRandomColor();
    }

    private void SetRandomColor() {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        Color color = new Color(r, g, b);

        _drawer?.SetColor(color);
    }
}
