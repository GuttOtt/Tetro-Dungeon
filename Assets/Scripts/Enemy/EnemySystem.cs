using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine.UIElements;
using Array2DEditor;
using Unity.VisualScripting;

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
    private List<CharacterBlockConfig> characterPool = new List<CharacterBlockConfig>();
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

    [SerializeField]
    private EnemyEffect _enemyEffect;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<IGameManager>();
        _unitSystem = _gameManager.GetSystem<UnitSystem>();

        _gameManager.GetSystem<BattleSystem>().OnBattleBegin += ActivateBattleBeginEffect;
        _gameManager.GetSystem<BattleSystem>().OnTimePass += EffectCoolDown;

        characterPool = Resources.LoadAll<CharacterBlockConfig>("Scriptable Objects/Character Block/Enemy").ToList();

        InitByStageData();
    }

    public void CreateEnemyUnits() {
        List<BaseUnit> units = new List<BaseUnit>();

        // ���� ����
        foreach (UnitPlacementConfig placementConfig in _enemyData.UnitPlacementConfigs) {
            CharacterBlockConfig characterBlockConfig = placementConfig.chracterBlockConfig;
            Array2DBool placableArea = placementConfig.placableArea;
            int placeAmount = placementConfig.placeAmount;
            int placeAmountPerDifficerty = (int) (placementConfig.placeAmountPerDifficerty * StageManager.Instance.CurrentStageIndex);
            placeAmount += placeAmountPerDifficerty;

            for (int i = 0; i < placeAmount; i++) {
                BaseUnit unit = _unitSystem.CreateUnit(characterBlockConfig, CharacterTypes.Enemy);
                PlaceUnit(unit, placableArea);
                units.Add(unit);
            }
        }

        // ��� ����

    }

    private void PlaceUnit(BaseUnit unit, Array2DBool placableArea) {
        Board board = _gameManager.GetSystem<Board>();
        List<Cell> availableCells = board.GetEmptyCellsInArea(placableArea.GetCells(), 0, board.Column / 2);
        
        if (availableCells.Count == 0) {
            Debug.LogError("EnemySystem: PlaceUnit - No available cell to place unit");
            return;
        }
        
        Cell cellToPlace = availableCells[Random.Range(0, availableCells.Count)];
        board.Place(cellToPlace, unit);
        unit.GetComponent<SpriteRenderer>().color = new Color(1, 0.5f, 0.5f);
    }


    public void SetDifficulty(int difficulty) {
        
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

    private void InitByStageData() {
        StageManager stageManager = StageManager.Instance;

        if (stageManager.CurrentEnemyData != null) {
            _enemyData = stageManager.CurrentEnemyData;
        }

        if (_enemyData == null) {
            Debug.LogError("EnemySystem�� EnemyData�� �Ҵ�Ǿ� ���� �ʽ��ϴ�. StageManager�� ���� EnemyData�� �ҷ� ���̰ų�, Ȥ�� �ν�����â�� ���� �Ҵ� �Ǿ� �־�� �մϴ�.");
        }

        _enemyEffect = _enemyData.EnemyEffect;
        characterPool = _enemyData.CharacterBlockConfigs;

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
