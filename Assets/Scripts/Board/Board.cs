using EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Board : MonoBehaviour
{
    #region private members
    //Manager and systems
    private IGameManager _gameManager;
    private UnitBlockSystem _unitBlockSystem;

    [SerializeField] private int row, column;
    [SerializeField] private Cell cellPrefab;
    private Cell[,] cells;
    private Dictionary<CharacterTypes, List<IUnit>> unitDic = new Dictionary<CharacterTypes, List<IUnit>>();

    //PathFinding
    private PathFinder _pathFinder;
    #endregion

    #region properties
    public List<IUnit> PlayerUnits { get => unitDic[CharacterTypes.Player]; }
    public List<IUnit> EnemyUnits { get => unitDic[CharacterTypes.Enemy]; }
    public int Row { get => row; }
    public int Column { get => column; }
    #endregion

    #region Events
    public event Action onPlaceUnit;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _unitBlockSystem = _gameManager.GetSystem<UnitBlockSystem>();
    }

    public void Init() {
        cells = new Cell[column, row];
        Vector3 cellSize = cellPrefab.GetComponent<SpriteRenderer>().bounds.size;

        //�� ��ġ
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

        //���� ��ųʸ� �ʱ�ȭ
        unitDic.Add(CharacterTypes.Player, new List<IUnit>());
        unitDic.Add(CharacterTypes.Enemy, new List<IUnit>());

        //�н� ���δ� ����
        _pathFinder = new PathFinder(column, row, false, true);
    }


    //polyomino�� cell�� ���� �� �ִ��� Ȯ��
    //��ġ ������ Top Left
    public bool IsPlacable(Polyomino polyomino, Cell cell, CharacterTypes characterType = CharacterTypes.Player) {
        int[,] shape = polyomino.Shape;
        int xTopLeft = cell.position.col;
        int yTopLeft = cell.position.row;

        //�������̳밡 board�� �ε����� ������� üũ
        if (column < xTopLeft + shape.GetLength(0)
            || row < yTopLeft + shape.GetLength(1)) {
            Debug.Log("�װ��� ������ ��ġ�� �� �����ϴ�");
            return false;
        }

        //�������� �ڸ��� ������ �ִ��� üũ
        for (int x = 0; x < shape.GetLength(0); x++) {
            for (int y = 0; y < shape.GetLength(1); y++) {
                Cell cellToCheck = cells[x + xTopLeft, y + yTopLeft];
                if (cellToCheck.CharacterType != characterType //ĳ���� Ÿ���� ���� �ʰų�
                    || (shape[x, y] == 1 && cellToCheck.Unit != null)) { //�̹� ������ ������
                    Debug.Log("�װ��� ������ ��ġ�� �� �����ϴ�");
                    return false;
                }
            }
        }

        return true;
    }

    //Place UnitBlock
    public UnitBlock Place(Cell topLeft, Polyomino polyomino, UnitConfig unitConfig, CharacterTypes characterType = CharacterTypes.Player) {
        if (!IsPlacable(polyomino, topLeft)) return null;

        int[,] shape = polyomino.Shape;
        int xTopLeft = topLeft.position.col;
        int yTopLeft = topLeft.position.row;

        UnitSystem unitSystem = _gameManager.GetSystem<UnitSystem>();

        List<Cell> cellsToPlace = new List<Cell>();
        List<IUnit> units = new List<IUnit>();

        for (int x = 0; x < shape.GetLength(0); x++) {
            for (int y = 0; y < shape.GetLength(1); y++) {
                if (shape[x, y] != 0) {
                    cellsToPlace.Add(cells[xTopLeft + x, yTopLeft + y]);
                }
            }
        }

        foreach (Cell cell in cellsToPlace) {
            BaseUnit unit = unitSystem.CreateUnit(unitConfig, characterType) as BaseUnit;
            Place(cell, unit);
            units.Add(unit);
        }

        UnitBlock unitBlock = _unitBlockSystem.CreateUnitBlock(cellsToPlace, units, polyomino, topLeft);

        onPlaceUnit.Invoke();

        return unitBlock;
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

    #region Getting Cell
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

    public List<Cell> GetNearbyCells(Cell cell, bool isContainingCenter = true) {
        int xCenter = cell.position.col;
        int yCenter = cell.position.row;

        int startCol = xCenter == 0 ? 0 : xCenter - 1;
        int endCol = xCenter == this.column - 1 ? this.column - 1 : xCenter + 1;
        int startRow = yCenter == 0 ? 0 : yCenter - 1;
        int endRow = yCenter == this.row - 1 ? this.row - 1 : yCenter + 1;

        List<Cell> nearbyCells = new List<Cell>();

        for (int i = startCol; i <= endCol; i++) {
            for (int j = startRow; j <= endRow; j++) {
                nearbyCells.Add(cells[i, j]);
            }
        }

        //�߽��� �������� �ʴ´ٸ�, �߽ɿ� �ִ� ���� ����
        if (!isContainingCenter)
            nearbyCells.Remove(cell);

        return nearbyCells;
    }

    public List<UnitBlock> GetNearbyBlocks(UnitBlock unitBlock) {
        List<UnitBlock> nearbyBlocks = new List<UnitBlock>();

        foreach (Cell cell in unitBlock.Cells) {
            List<Cell> nearbyCells = GetNearbyCells(cell, false);

            foreach (Cell nearbyCell in nearbyCells) {
                UnitBlock nearbyBlock = _unitBlockSystem.GetUnitBlock(nearbyCell);

                if (nearbyBlock != null && !nearbyBlocks.Contains(nearbyBlock) && nearbyBlock != unitBlock) {
                    nearbyBlocks.Add(nearbyBlock);
                }
            }
        }

        return nearbyBlocks;
    }

    public List<Cell> GetCellsInRange(Cell center, int minRange, int maxRange) {
        List<Cell> cellsInRange = new List<Cell>();

        foreach (Cell cell in cells) {
            float distance = GetDistance(center, cell);

            if (minRange <= distance && distance <= maxRange) {
                cellsInRange.Add(cell);
            }
        }

        return cellsInRange;
    }
    #endregion

    #region Getting Unit
    public List<IUnit> GetUnits(CharacterTypes chracterType) {
        return unitDic[chracterType].ToList();
    }

    /// <summary>
    /// center Cell�� �������� ���� ����� characterType ������ ��ȯ
    /// maxDistance�� üũ�ϴ� �ִ� �Ÿ�
    /// �����¿� �� ĭ�� �Ÿ��� 1, �밢���� ��Ʈ 2�� ������
    /// </summary>
    public IUnit GetClosestUnit(Cell center, CharacterTypes characterType, int maxDistance) {
        List<IUnit> units = GetUnits(characterType);

        float temp = maxDistance;
        IUnit closest = null;

        foreach (IUnit unit in units) {
            float distance = GetDistance(center, unit.CurrentCell);

            if (distance <= temp) {
                temp = distance;
                closest = unit;
            }
        }

        return closest;
    }
    #endregion

    private float GetDistance(Cell cell1, Cell cell2) {
        float x = cell1.position.col - cell2.position.col;
        float y = cell1.position.row - cell2.position.row;

        return Mathf.Sqrt(Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f));
    }

    #region Pathfinding
    public List<Cell> PathFinding(Cell startCell, Cell targetCell) {
        List<Node> nodes =
            _pathFinder.PathFinding(startCell.position.col, startCell.position.row, targetCell.position.col, targetCell.position.row, cells);

        List<Cell> cellPath = new List<Cell>();

        foreach (Node node in nodes) {
            cellPath.Add(cells[node.x, node.y]);
        }

        return cellPath;
    }
    #endregion
}

[Serializable]
public class PathFinder {
    private Node _startNode, _targetNode, _curNode;
    private bool _allowDiagonal, _dontCrossCorner;
    private int _sizeX, _sizeY;


    List<Node> OpenList, ClosedList;
    Node[,] NodeArray;

    public PathFinder(int sizeX, int sizeY, bool allowDiagonal, bool dontCrossCorner) {
        _sizeX = sizeX;
        _sizeY = sizeY;
        _allowDiagonal = allowDiagonal;
        _dontCrossCorner = dontCrossCorner;
    }

    public void InitMap(Cell[,] cells) {
        _sizeX = cells.GetLength(0);
        _sizeY = cells.GetLength(1);
        NodeArray = new Node[_sizeX, _sizeY];

        for (int i = 0; i < _sizeX; i++) {
            for (int j = 0; j < _sizeY; j++) {
                bool isWall = false;

                if (cells[i, j].Unit != null) isWall = true;

                NodeArray[i, j] = new Node(isWall, i, j);
            }
        }
    }

    public List<Node> PathFinding(int startX, int startY, int targetX, int targetY, Cell[,] map) {
        InitMap(map);

        List<Node> FinalNodeList = new List<Node>();

        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        _startNode = NodeArray[startX, startY];
        _targetNode = NodeArray[targetX, targetY];

        OpenList = new List<Node>() { _startNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (OpenList.Count > 0) {
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            _curNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= _curNode.F && OpenList[i].H < _curNode.H) _curNode = OpenList[i];

            OpenList.Remove(_curNode);
            ClosedList.Add(_curNode);


            // ������
            if (_curNode == _targetNode) {
                Node TargetCurNode = _targetNode;
                while (TargetCurNode != _startNode) {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(_startNode);
                FinalNodeList.Reverse();

                return FinalNodeList;
            }


            // �֢آע�
            if (_allowDiagonal) {
                OpenListAdd(_curNode.x + 1, _curNode.y + 1);
                OpenListAdd(_curNode.x - 1, _curNode.y + 1);
                OpenListAdd(_curNode.x - 1, _curNode.y - 1);
                OpenListAdd(_curNode.x + 1, _curNode.y - 1);
            }

            // �� �� �� ��
            OpenListAdd(_curNode.x, _curNode.y -1);
            OpenListAdd(_curNode.x + 1, _curNode.y);
            OpenListAdd(_curNode.x, _curNode.y +1);
            OpenListAdd(_curNode.x - 1, _curNode.y);
        }

        return FinalNodeList;
    }

    void OpenListAdd(int checkX, int checkY) {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= 0 && checkX < _sizeX && checkY >= 0 && checkY < _sizeY && !NodeArray[checkX, checkY].isWall && !ClosedList.Contains(NodeArray[checkX, checkY])) {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (_allowDiagonal) {
                if (NodeArray[_curNode.x, checkY].isWall && NodeArray[checkX, _curNode.y].isWall) {
                    Debug.Log("Through Walls");
                    return;
                }
            }

            /*
            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (_dontCrossCorner) {
                if (NodeArray[_curNode.x, checkY].isWall || NodeArray[checkX, _curNode.y].isWall) {
                    Debug.Log("Crossing Corner");
                    return;
                }
            }
            */

            // �̿���忡 �ְ�, ������ 10, �밢���� 14���
            Node NeighborNode = NodeArray[checkX, checkY];
            int MoveCost = _curNode.G + (_curNode.x - checkX == 0 || _curNode.y - checkY == 0 ? 10 : 14);


            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode)) {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - _targetNode.x) + Mathf.Abs(NeighborNode.y - _targetNode.y)) * 10;
                NeighborNode.ParentNode = _curNode;


                OpenList.Add(NeighborNode);
            }
        }
    }
}

public class Node {
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}