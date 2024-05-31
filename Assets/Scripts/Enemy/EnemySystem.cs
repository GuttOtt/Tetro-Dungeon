using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Runtime.CompilerServices;
using System.Linq;

public class EnemySystem : MonoBehaviour {
    #region private members
    //Manager and Systems
    private IGameManager _gameManager;
    private UnitSystem _unitSystem;

    //Unit ����
    [SerializeField]
    private List<UnitConfig> unitPool = new List<UnitConfig>();
    private List<BaseUnit> unitList = new List<BaseUnit>();//�̹� �Ͽ� ��ȯ�� ���ֵ�

    [SerializeField]
    private GameObject unitListDisplay;

    [SerializeField]
    private Vector2 unitGap; //unitListDisplay ���̿� ǥ�õǴ� unit ������ ��

    [SerializeField]
    private int unitAmount = 5; //�� �Ͽ� ��ȯ�Ǵ� ������ ��(�ӽ�)
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<IGameManager>();
        _unitSystem = _gameManager.GetSystem<UnitSystem>();
    }

    public void DecideUnitList() {
        DecideUnitList(unitAmount);
    }

    public void DecideUnitList(int number) {
        ClearUnitList();

        for (int i = 0; i < number; i++) {
            int r = Random.Range(0, unitPool.Count);
            UnitConfig config = unitPool[r];
            BaseUnit unit = _unitSystem.CreateUnit(config, CharacterTypes.Enemy) as BaseUnit;
            //unit.GetComponent<SpriteRenderer>().color = Color.red;
            
            //������ unitList�� �߰�
            unitList.Add(unit);
            unit.transform.parent = unitListDisplay.transform;
            unit.transform.localPosition = unitGap * i;
        }
    }

    public void PlaceUnit() {
        Board board = _gameManager.GetSystem<Board>();
        List<Cell> availableCells = board.GetAllEmptyCell(CharacterTypes.Enemy);

        foreach (BaseUnit unit in unitList) {
            //������ �� �� �������� �ϳ� ����
            int r = Random.Range(0, availableCells.Count);
            Cell cell = availableCells[r];

            //������ ���� ���� ��ȯ
            board.Place(cell, unit);

            availableCells.Remove(cell);
        }
    }

    private void ClearUnitList() {
        unitList.Clear();
    }
}
