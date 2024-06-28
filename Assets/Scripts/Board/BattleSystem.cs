using Cysharp.Threading.Tasks;
using EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    private Board _board;
    private SynergySystem _synergySystem;

    //배틀이 진행중인가?
    private bool _isProcessing = false;

    //현재 공격 턴
    private CharacterTypes _attackTurn;

    //라이프 (이후 다른 클래스로 옮겨야 할 수도 있음)
    [SerializeField] private int _playerMaxLife, _enemyMaxLife;
    private Dictionary<CharacterTypes, int> _lifeDic;
    [SerializeField] private TMP_Text _playerLifeText;
    [SerializeField] private TMP_Text _enemyLifeText;
    private Dictionary<CharacterTypes, TMP_Text> _lifeTextDic = new Dictionary<CharacterTypes, TMP_Text>();

    //배틀 UniTask를 중단하기 위한 CancellationToken
    private CancellationTokenSource battleCancel = new CancellationTokenSource();

    [SerializeField]
    private float _battleSpeed = 1; //1이 기준
    #endregion

    #region Events
    public event Action OnStartBattle;
    #endregion

    #region
    public bool IsProcessing { get => _isProcessing; }
    public CharacterTypes AttackTurn { get => _attackTurn; }
    #endregion


    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
        _synergySystem = _gameManager.GetSystem<SynergySystem>();

        //Set Life
        _lifeDic = new Dictionary<CharacterTypes, int>() { 
            { CharacterTypes.Player, _playerMaxLife },
            { CharacterTypes.Enemy, _enemyMaxLife }
        };

        _lifeTextDic.Add(CharacterTypes.Player, _playerLifeText);
        _lifeTextDic.Add(CharacterTypes.Enemy, _enemyLifeText);
        UpdateLifeText(CharacterTypes.Player);
        UpdateLifeText(CharacterTypes.Enemy);
    }

    public async UniTask StartBattle() {
        if (_isProcessing)
            return;

        OnStartBattle.Invoke();

        Debug.Log("Battle Started");
        _isProcessing = true;
        CharacterTypes winner = CharacterTypes.None;

        //배틀 시작 시 발생하는 Synergy 효과들 발동
        await _synergySystem.OnBattleBeginEffects((_gameManager as GameManager).CreateTurnContext());

        //한 쪽의 유닛이 전부 사라질 때까지 전투
        while (true) {
            if (_board.GetUnits(CharacterTypes.Enemy).Count == 0) {
                winner = CharacterTypes.Player;
                break;
            }
            else if (_board.GetUnits(CharacterTypes.Player).Count == 0) {
                winner = CharacterTypes.Enemy;
                break;
            }

            //틱 진행
            TimePass();

            await UniTask.NextFrame();
        }

        //남은 승리 유닛들의 공격력만큼 패배한 캐릭터에게 데미지
        List<IUnit> winnerUnits = _board.GetUnits(winner);
        foreach (IUnit unit in winnerUnits) {
            LifeDamage(winner.Opponent(), unit.Attack);
            (unit as BaseUnit).Die();
            await UniTask.WaitForSeconds(0.5f);
        }

        //배틀 종료
        _isProcessing= false;
        _attackTurn = CharacterTypes.None;
        _gameManager.GetSystem<PhaseSystem>().ToEndPhase();
    }

    public void TimePass() {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();

        foreach (IUnit unit in playerUnits) {
            if (unit is null || unit as BaseUnit is null) continue;

            BaseUnit baseUnit = unit as BaseUnit;
            baseUnit.ActionCoolDown(Time.deltaTime * _battleSpeed);
                        
            if (!baseUnit.IsActionCoolDown) {
                baseUnit.Act(_gameManager.CreateTurnContext());
            }
        }

        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenBy(unit => unit.CurrentCell.position.col).ToList();

        foreach (IUnit unit in enemyUnits) {
            if (unit is null || unit as BaseUnit is null) continue;

            BaseUnit baseUnit = unit as BaseUnit;
            baseUnit.ActionCoolDown(Time.deltaTime * _battleSpeed);

            if (!baseUnit.IsActionCoolDown) {
                baseUnit.Act(_gameManager.CreateTurnContext());
            }
        }
    }

    private void StopBattle() {

    }

    private bool CheckMovable(IUnit unit, CharacterTypes attackTurn) {
        if (unit.Owner != attackTurn) {
            return false;
        }
        Cell currentCell = unit.CurrentCell;

        // 전방으로 한 칸의 위치 계산
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //유닛의 앞쪽 셀을 가져옴
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //forwardCell에 이미 유닛이 있거나, forwardCell이 존재하지 않는다면 false
        if (forwardCell == null || forwardCell.Unit != null)
            return false;

        return true;
    }


    private void ProcessDeath(ref List<IUnit> playerUnits, ref List<IUnit> enemyUnits) {
        playerUnits = _board.PlayerUnits.ToList();
        enemyUnits = _board.EnemyUnits.ToList();
    }

    #region Life Damage
    private void LifeDamage(CharacterTypes characterType, int damage) {
        _lifeDic[characterType] -= damage;
        UpdateLifeText(characterType);
    }

    private void UpdateLifeText(CharacterTypes charactertype) {
        _lifeTextDic[charactertype].text = "Life: " + _lifeDic[charactertype].ToString();
    }
    #endregion
}
