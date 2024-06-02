using EnumTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    [SerializeField] private int row, column;
    [SerializeField] private Cell cellPrefab;
    private Cell[,] cells;
    private Dictionary<CharacterTypes, List<IUnit>> unitDic = new Dictionary<CharacterTypes, List<IUnit>>();
    #endregion

    #region properties
    public List<IUnit> PlayerUnits { get => unitDic[CharacterTypes.Player]; }
    public List<IUnit> EnemyUnits { get => unitDic[CharacterTypes.Enemy]; }
    public int Row { get => row; }
    public int Column { get => column; }
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

    public void Init() {
        cells = new Cell[column, row];
        Vector3 cellSize = cellPrefab.GetComponent<SpriteRenderer>().bounds.size;

        //셀 배치
        for (int x = 0; x < column; x++) {
            for (int y = 0; y < row; y++) {
                cellPrefab.name = $"({x},{y})";
                Cell cell = Instantiate(cellPrefab, this.transform);
                cells[x, y] = cell;
                cells[x, y].transform.localPosition = new Vector2(cellSize.x * x, -cellSize.y * y);

                if (x < column / 2) {
                    cell.Init(x, y, CharacterTypes.Player);
                    cell.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                else {
                    cell.Init(x, y, CharacterTypes.Enemy);
                    cell.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }

        //유닛 딕셔너리 초기화
        unitDic.Add(CharacterTypes.Player, new List<IUnit>());
        unitDic.Add(CharacterTypes.Enemy, new List<IUnit>());
    }


    //polyomino를 cell에 놓을 수 있는지 확인
    //위치 기준은 Top Left
    public bool IsPlacable(Polyomino polyomino, Cell cell, CharacterTypes characterType = CharacterTypes.Player) {
        int[,] shape = polyomino.Shape;
        int xTopLeft = cell.position.col;
        int yTopLeft = cell.position.row;

        //폴리오미노가 board의 인덱스를 벗어나는지 체크
        if (column < xTopLeft + shape.GetLength(0)
            || row < yTopLeft + shape.GetLength(1)) {
            Debug.Log("그곳에 유닛을 배치할 수 없습니다");
            return false;
        }

        //놓으려는 자리에 유닛이 있는지 체크
        for (int x = 0; x < shape.GetLength(0); x++) {
            for (int y = 0; y < shape.GetLength(1); y++) {
                Cell cellToCheck = cells[x + xTopLeft, y + yTopLeft];
                if (cellToCheck.CharacterType != characterType //캐릭터 타입이 맞지 않거나
                    || (shape[x, y] == 1 && cellToCheck.Unit != null)) { //이미 유닛이 있으면
                    Debug.Log("그곳에 유닛을 배치할 수 없습니다");
                    return false;
                }
            }
        }

        return true;
    }

    //Place UnitBlock
    public bool Place(Cell topLeft, Polyomino polyomino, UnitConfig unitConfig, CharacterTypes characterType = CharacterTypes.Player) {
        if (!IsPlacable(polyomino, topLeft)) return false;

        int[,] shape = polyomino.Shape;
        int xTopLeft = topLeft.position.col;
        int yTopLeft = topLeft.position.row;

        UnitSystem unitSystem = _gameManager.GetSystem<UnitSystem>();

        for (int x = 0; x < shape.GetLength(0); x++) {
            for (int y = 0; y < shape.GetLength(1); y++) {
                if (shape[x, y] == 0) continue;
                Cell cell = cells[x + xTopLeft, y + yTopLeft];
                BaseUnit unit = unitSystem.CreateUnit(unitConfig, characterType) as BaseUnit;
                Place(cell, unit);
            }
        }

        return true;
    }

    //Place a Unit
    public bool Place(Cell cell, BaseUnit unit) {
        if (cell.Unit != null) {
            return false;
        }

        cell.UnitIn(unit);
        unit.CurrentCell = cell;
        unit.transform.parent = cell.transform;
        unit.transform.localPosition = Vector3.zero;
        unitDic[unit.Owner].Add(unit);

        unit.OnDie += () => unitDic[unit.Owner].Remove(unit);

        return true;
    }

    public List<IUnit> GetUnits(CharacterTypes chracterType) {
        return unitDic[chracterType].ToList();
    }

    public Cell GetCell(int col, int row) {
        if (col < 0 || this.column <= col || row < 0 || this.row <= row) {
            return null;
        }
        return cells[col, row];
    }

    public List<Cell> GetAllEmptyCell(CharacterTypes owner) {
        List<Cell> allCell = new List<Cell>();

        foreach (Cell cell in cells) {
            if (cell.Unit == null && cell.CharacterType == owner) {
                allCell.Add(cell);
            }
        }

        return allCell.ToList();
    }
}
