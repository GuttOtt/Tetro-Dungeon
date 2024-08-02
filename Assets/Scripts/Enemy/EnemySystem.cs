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

    //Unit 관련
    [SerializeField]
    private List<UnitConfig> unitPool = new List<UnitConfig>();
    private List<BaseUnit> unitList = new List<BaseUnit>();//이번 턴에 소환할 유닛들

    [SerializeField]
    private GameObject unitListDisplay;

    [SerializeField]
    private Vector2 unitGap; //unitListDisplay 사이에 표시되는 unit 사이의 갭

    [SerializeField]
    private int unitAmount = 5; //한 턴에 소환되는 유닛의 수(임시)

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
            //유닛 풀에서 랜덤으로 하나를 선택 후 Create
            int r = Random.Range(0, unitPool.Count);
            UnitConfig config = unitPool[r];
            BaseUnit unit = _unitSystem.CreateUnit(config, CharacterTypes.Enemy) as BaseUnit;

            unit.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);
            
            //유닛을 unitList에 추가
            unitList.Add(unit);
            unit.transform.parent = unitListDisplay.transform;
            unit.transform.localPosition = unitGap * i;
        }
    }

    public void PlaceUnit() {
        Board board = _gameManager.GetSystem<Board>();
        List<Cell> availableCells = board.GetAllEmptyCell(CharacterTypes.Enemy);

        foreach (BaseUnit unit in unitList) {
            //가능한 셀 중 랜덤으로 하나 선택
            int r = Random.Range(0, availableCells.Count);
            Cell cell = availableCells[r];

            //선택한 셀에 유닛 소환
            board.Place(cell, unit);
            unit.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);

            availableCells.Remove(cell);
        }
    }

    public void SetDifficulty(int difficulty) {
        _buffTokensPerRound.Clear();

        //난이도에 따라 버프 토큰의 수를 조정
        for (int i = 0; i < difficulty; i++) {
            _buffTokensPerRound.AddRange(_enemyData.BuffTokenPerDifficulty);
        }

        //만약 유닛이 있다면, 초기화 하고 난이도에 맞게 다시 소환
        if (unitList != null && 0 < unitList.Count) {
            
        }
    }

    private void ClearUnitList() {
        if (unitList != null && 0 < unitList.Count) {
            List<BaseUnit> temp = unitList.ToList();

            foreach (BaseUnit unit in temp) {
                if (unit != null && unit.gameObject != null) {
                    unit.Die(); // Die로 해도 괜찮을까? 아예 제거되는 별도의 메소드가 필요하지 않을까?
                }
            }
        }

        unitList.Clear();
    }
}
