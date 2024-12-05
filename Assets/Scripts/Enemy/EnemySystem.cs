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
    private List<CharacterBlockConfig> characterPool = new List<CharacterBlockConfig>();
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

    [SerializeField]
    private EnemyEffect _enemyEffect;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<IGameManager>();
        _unitSystem = _gameManager.GetSystem<UnitSystem>();

        _gameManager.GetSystem<BattleSystem>().OnBattleBegin += ActivateBattleBeginEffect;
        _gameManager.GetSystem<BattleSystem>().OnTimePass += EffectCoolDown;

        unitPool = Resources.LoadAll<UnitConfig>("Scriptable Objects/Unit").ToList();
        characterPool = Resources.LoadAll<CharacterBlockConfig>("Scriptable Objects/Character Block").ToList();

        InitByStageData();
    }

    public void DecideUnitList() {
        DecideUnitList(unitAmount);
    }

    public void DecideUnitList(int number) {
        ClearUnitList();

        for (int i = 0; i < number; i++) {
            //유닛 풀에서 랜덤으로 하나를 선택 후 Create
            int r = Random.Range(0, characterPool.Count);
            CharacterBlockConfig config = characterPool[r];
            BaseUnit unit = _unitSystem.CreateUnit(config, CharacterTypes.Enemy);

            unit.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);
            
            //유닛을 unitList에 추가
            unitList.Add(unit);
        }

        PlaceUnit();
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
            DecideUnitList();
        }
    }

    private void ApplyBuffTokens() {
        Dictionary<BaseUnit, List<BuffToken>> unitBuffDic = new Dictionary<BaseUnit, List<BuffToken>>();

        foreach (BaseUnit unit in unitList) {
            unitBuffDic.Add(unit, new List<BuffToken>());
        }

        foreach (BuffToken token in _buffTokensPerRound) {
            BaseUnit unit = unitList[Random.Range(0, unitList.Count)];

            unitBuffDic[unit].Add(token);
        }

        foreach (BaseUnit unit in unitBuffDic.Keys) {
            float attackBuff = 1;
            float maxHPBuff = 1;

            foreach (BuffToken token in unitBuffDic[unit]) {
                attackBuff += token.Attack;
                maxHPBuff += token.MaxHP;
            }

            int newAttack = (int) (unit.Attack * attackBuff);
            int newMaxHP = (int)(unit.MaxHP * maxHPBuff);

            unit.SetAttack(newAttack);
            unit.SetMaxHP(newMaxHP);
        }
    }

    private void ClearUnitList() {
        if (unitList != null && 0 < unitList.Count) {
            List<BaseUnit> temp = unitList.ToList();

            foreach (BaseUnit unit in temp) {
                if (unit != null && unit.gameObject != null) {
                    unit.DestroySelf();
                }
            }
        }

        unitList.Clear();
    }

    private void InitByStageData() {
        StageManager stageManager = StageManager.Instance;

        if (stageManager.CurrentEnemyData != null) {
            _enemyData = stageManager.CurrentEnemyData;
        }

        if (_enemyData == null) {
            Debug.LogError("EnemySystem에 EnemyData가 할당되어 있지 않습니다. StageManager를 통해 EnemyData를 불러 들이거나, 혹은 인스펙터창을 통해 할당 되어 있어야 합니다.");
        }

        _enemyEffect = _enemyData.EnemyEffect;

        unitPool = _enemyData.UnitConfigs;

        SetDifficulty(stageManager.CurrentStageIndex);

        Debug.Log($"Enemy Name: {_enemyData.Name}");
        Debug.Log($"Difficulty: {stageManager.CurrentStageIndex}");
    }

    #region Enemy Effect
    private void EffectCoolDown() {
        if (_enemyEffect == null || !_enemyEffect.IsCoolDownEffect)
            return;

        _enemyEffect.CoolDownCount += Time.deltaTime;
        
        if (_enemyEffect.CoolTime <= _enemyEffect.CoolDownCount) {
            _enemyEffect.CoolTimeEffect(_gameManager.CreateTurnContext());
            _enemyEffect.CoolDownCount = 0;
        }
    }

    private void ActivateBattleBeginEffect() {

        if (_enemyEffect == null || !_enemyEffect.IsOnBattleBegin)
            return;

        _enemyEffect.OnBattleBeginEffect(_gameManager.CreateTurnContext());
    }
    #endregion
}
