using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine.UIElements;

public class EnemySystem : MonoBehaviour {
    #region private members
    //Manager and Systems
    private IGameManager _gameManager;
    private UnitSystem _unitSystem;

    [SerializeField]
    private EnemyData _enemyData;

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

    //Buff Token
    [SerializeField]
    private List<BuffToken> _buffTokensPerRound = new List<BuffToken>();
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<IGameManager>();
        _unitSystem = _gameManager.GetSystem<UnitSystem>();

        unitPool = Resources.LoadAll<UnitConfig>("Scriptable Objects/Unit").ToList();
    }

    public void DecideUnitList() {
        DecideUnitList(unitAmount);
    }

    public void DecideUnitList(int number) {
        ClearUnitList();

        for (int i = 0; i < number; i++) {
            //���� Ǯ���� �������� �ϳ��� ���� �� Create
            int r = Random.Range(0, unitPool.Count);
            UnitConfig config = unitPool[r];
            BaseUnit unit = _unitSystem.CreateUnit(config, CharacterTypes.Enemy) as BaseUnit;

            unit.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);
            
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
            unit.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);

            availableCells.Remove(cell);
        }
    }

    public void SetDifficulty(int difficulty) {
        _buffTokensPerRound.Clear();

        //���̵��� ���� ���� ��ū�� ���� ����
        for (int i = 0; i < difficulty; i++) {
            _buffTokensPerRound.AddRange(_enemyData.BuffTokenPerDifficulty);
        }

        //���� ������ �ִٸ�, �ʱ�ȭ �ϰ� ���̵��� �°� �ٽ� ��ȯ
        if (unitList != null && 0 < unitList.Count) {
            
        }
    }

    private void ClearUnitList() {
        if (unitList != null && 0 < unitList.Count) {
            List<BaseUnit> temp = unitList.ToList();

            foreach (BaseUnit unit in temp) {
                if (unit != null && unit.gameObject != null) {
                    unit.Die(); // Die�� �ص� ��������? �ƿ� ���ŵǴ� ������ �޼ҵ尡 �ʿ����� ������?
                }
            }
        }

        unitList.Clear();
    }
}
