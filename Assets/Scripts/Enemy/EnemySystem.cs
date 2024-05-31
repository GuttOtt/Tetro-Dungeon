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

            availableCells.Remove(cell);
        }
    }

    private void ClearUnitList() {
        unitList.Clear();
    }
}
