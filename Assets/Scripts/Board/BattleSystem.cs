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

    //��Ʋ�� �������ΰ�?
    private bool _isProcessing = false;

    //���� ���� ��
    private CharacterTypes _attackTurn;

    //������ (���� �ٸ� Ŭ������ �Űܾ� �� ���� ����)
    [SerializeField] private int _playerMaxLife, _enemyMaxLife;
    private Dictionary<CharacterTypes, int> _lifeDic;
    [SerializeField] private TMP_Text _playerLifeText;
    [SerializeField] private TMP_Text _enemyLifeText;
    private Dictionary<CharacterTypes, TMP_Text> _lifeTextDic = new Dictionary<CharacterTypes, TMP_Text>();

    //��Ʋ UniTask�� �ߴ��ϱ� ���� CancellationToken
    private CancellationTokenSource battleCancel = new CancellationTokenSource();

    [SerializeField]
    private float _battleSpeed = 1; //1�� ����
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

        //��Ʋ ���� �� �߻��ϴ� Synergy ȿ���� �ߵ�
        await _synergySystem.OnBattleBeginEffects((_gameManager as GameManager).CreateTurnContext());

        //�� ���� ������ ���� ����� ������ ����
        while (true) {
            if (_board.GetUnits(CharacterTypes.Enemy).Count == 0) {
                winner = CharacterTypes.Player;
                break;
            }
            else if (_board.GetUnits(CharacterTypes.Player).Count == 0) {
                winner = CharacterTypes.Enemy;
                break;
            }

            //ƽ ����
            TimePass();

            await UniTask.NextFrame();
        }

        //���� �¸� ���ֵ��� ���ݷ¸�ŭ �й��� ĳ���Ϳ��� ������
        List<IUnit> winnerUnits = _board.GetUnits(winner);
        foreach (IUnit unit in winnerUnits) {
            LifeDamage(winner.Opponent(), unit.Attack);
            (unit as BaseUnit).Die();
            await UniTask.WaitForSeconds(0.5f);
        }

        //��Ʋ ����
        _isProcessing= false;
        _attackTurn = CharacterTypes.None;
        _gameManager.GetSystem<PhaseSystem>().ToEndPhase();
    }

    public void TimePass() {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();

        //���� row ������, ���� row������ ����(���� ��) ���� ������ ����
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

        //���� row ������, ���� row������ ����(���� ��) ���� ������ ����
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

        // �������� �� ĭ�� ��ġ ���
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //������ ���� ���� ������
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //forwardCell�� �̹� ������ �ְų�, forwardCell�� �������� �ʴ´ٸ� false
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
