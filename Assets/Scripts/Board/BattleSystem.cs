using Assets.Scripts;
using Cysharp.Threading.Tasks;
using EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
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

    //적 이름
    [SerializeField] private TMP_Text _enemyNameText;

    //라이프 (이후 다른 클래스로 옮겨야 할 수도 있음)
    [SerializeField] private int _playerMaxLife, _enemyMaxLife;
    private Dictionary<CharacterTypes, int> _lifeDic;
    [SerializeField] private TMP_Text _playerLifeText;
    [SerializeField] private TMP_Text _enemyLifeText;
    private Dictionary<CharacterTypes, TMP_Text> _lifeTextDic = new Dictionary<CharacterTypes, TMP_Text>();

    //배틀 UniTask를 중단하기 위한 CancellationToken
    private CancellationTokenSource battleCancellationToken = new CancellationTokenSource();

    [SerializeField]
    private float _battleSpeed = 1; //1이 디폴트
    #endregion

    #region Events
    public event Action OnBattleBegin;
    public event Action OnTimePass;
    #endregion

    #region
    public bool IsProcessing { get => _isProcessing; }
    public CharacterTypes AttackTurn { get => _attackTurn; }
    #endregion


    private void Awake()
    {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
        _synergySystem = _gameManager.GetSystem<SynergySystem>();
    }

    public async UniTask StartBattle()
    {
        battleCancellationToken = new CancellationTokenSource();

        if (_isProcessing)
            return;

        Debug.Log("Battle Started");

        OnBattleBegin?.Invoke();

        _isProcessing = true;
        CharacterTypes winner = CharacterTypes.None;

        //Trigger OnStartBattle of Units
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();
        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();
        foreach (BaseUnit unit in playerUnits) {
            unit.TriggerOnBattleStart(_gameManager.CreateTurnContext());
        }
        foreach (BaseUnit unit in enemyUnits) {
            unit.TriggerOnBattleStart(_gameManager.CreateTurnContext());
        }

        //한 쪽의 유닛이 전부 사라질 때까지 전투
        while (true)
        {
            if (battleCancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (_board.GetUnits(CharacterTypes.Enemy).Count == 0)
            {
                winner = CharacterTypes.Player;
                break;
            }
            else if (_board.GetUnits(CharacterTypes.Player).Count == 0)
            {
                winner = CharacterTypes.Enemy;
                Debug.Log("Player Lose");
                break;
            }

            //틱 진행
            TimePass();

            await UniTask.NextFrame();
        }

        //배틀 승리 판정
        await UniTask.WaitForSeconds(2f);
        List<IUnit> winnerUnits = _board.GetUnits(winner);
        foreach (IUnit unit in winnerUnits)
        {
            (unit as BaseUnit).DestroySelf();
            await UniTask.WaitForSeconds(0.1f);
        }

        if (winner == CharacterTypes.Player) {
            _gameManager.PlayerWin();
            return;
        }
        else { 
            _gameManager.PlayerLose();
            return;
        }
    }

    public void TimePass() {
        OnTimePass?.Invoke();

        //시너지의 쿨타임 회복
        //_synergySystem.OnTimePass(_gameManager.CreateTurnContext());

        List<IUnit> playerUnits = _board.PlayerUnits.ToList();

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();

        foreach (IUnit unit in playerUnits)
        {
            if (unit is null || unit as BaseUnit is null) continue;

            BaseUnit baseUnit = unit as BaseUnit;
            baseUnit.ActionCoolDown(Time.deltaTime * _battleSpeed);

            if (!baseUnit.IsActionCoolDown)
            {
                baseUnit.Act(_gameManager.CreateTurnContext());
            }
        }

        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenBy(unit => unit.CurrentCell.position.col).ToList();

        foreach (IUnit unit in enemyUnits)
        {
            if (unit is null || unit as BaseUnit is null) continue;

            BaseUnit baseUnit = unit as BaseUnit;
            baseUnit.ActionCoolDown(Time.deltaTime * _battleSpeed);

            if (!baseUnit.IsActionCoolDown)
            {
                baseUnit.Act(_gameManager.CreateTurnContext());
            }
        }
    }

    private void StopBattle()
    {
        battleCancellationToken.Cancel();
        battleCancellationToken.Dispose();
    }

    private void OnDestroy()
    {
        if (battleCancellationToken != null)
        {
            StopBattle();
        }   
    }

  private bool CheckMovable(IUnit unit, CharacterTypes attackTurn)
    {
        if (unit.Owner != attackTurn)
        {
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


    private void ProcessDeath(ref List<IUnit> playerUnits, ref List<IUnit> enemyUnits)
    {
        playerUnits = _board.PlayerUnits.ToList();
        enemyUnits = _board.EnemyUnits.ToList();
    }
}
