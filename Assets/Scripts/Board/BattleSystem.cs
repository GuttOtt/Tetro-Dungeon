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
    
    //��Ʋ ���� ���� ������
    [SerializeField]
    private float delayPerUnit = 0.5f;
    [SerializeField]
    private float delayPerTick = 1f;

    //��Ʋ�� �������ΰ�?
    private bool _isProcessing = false;

    //������ (���� �ٸ� Ŭ������ �Űܾ� �� ���� ����)
    private Dictionary<CharacterTypes, int> _lifeDic;
    [SerializeField] private TMP_Text _playerLifeText;
    [SerializeField] private TMP_Text _enemyLifeText;
    private Dictionary<CharacterTypes, TMP_Text> _lifeTextDic = new Dictionary<CharacterTypes, TMP_Text>();

    //��Ʋ UniTask�� �ߴ��ϱ� ���� CancellationToken
    private CancellationTokenSource battleCancel = new CancellationTokenSource();
    #endregion

    #region Events
    #endregion

    private enum UnitActionTypes {
        Move, Attack, None
    }

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
        _synergySystem = _gameManager.GetSystem<SynergySystem>();

        //Set Life
        _lifeDic = new Dictionary<CharacterTypes, int>() { 
            { CharacterTypes.Player, 15 },
            { CharacterTypes.Enemy, 15 }
        };

        _lifeTextDic.Add(CharacterTypes.Player, _playerLifeText);
        _lifeTextDic.Add(CharacterTypes.Enemy, _enemyLifeText);
        UpdateLifeText(CharacterTypes.Player);
        UpdateLifeText(CharacterTypes.Enemy);
    }

    public async UniTask StartBattle(CharacterTypes attackTurn) {
        if (_isProcessing)
            return;

        Debug.Log("Battle Started");
        _isProcessing = true;

        //��Ʋ ���� �� �߻��ϴ� Synergy ȿ���� �ߵ�
        await _synergySystem.OnBattleBeginEffects(new TurnContext(_board, attackTurn));

        //Attack Turn�� ������ ���� ����� ������ ����
        while (true) {
            if (_board.GetUnits(attackTurn).Count == 0) break;

            //ƽ ����
            await ComputeTick(attackTurn);

            //���ʹ� ü���� 0 ���϶�� �÷��̾� �¸� ����
            if (_lifeDic[CharacterTypes.Enemy] <= 0) {
                _gameManager.PlayerWin();
                _isProcessing = false;
                return;
            }
        }

        //���� ��� ���ֵ��� ���ݷ¸�ŭ ������ ĳ���Ϳ��� ������
        List<IUnit> defenceUnits = _board.GetUnits(attackTurn.Opponent());
        foreach (IUnit unit in defenceUnits) {
            LifeDamage(attackTurn, unit.Attack);
            (unit as BaseUnit).Die();
            await UniTask.WaitForSeconds(0.5f);
        }

        //�÷��̾� ü���� 0 ���϶�� �÷��̾� �й� ����
        if (_lifeDic[CharacterTypes.Player] <= 0) {
            _gameManager.PlayerLose();
            _isProcessing = false;
            return;
        }

        //��Ʋ ����
        _isProcessing= false;
        _gameManager.GetSystem<PhaseSystem>().ToEndPhase();
    }

    public async UniTask ComputeTick(CharacterTypes attackTurn) {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();
        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //���� row ������, ���� row������ ����(���� ��) ���� ������ ����
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenBy(unit => unit.CurrentCell.position.col).ToList();

        TurnContext turnContext = new TurnContext(_board, attackTurn);

        //ƽ ���� �� �ߵ��ϴ� �ó��� ȿ���� �ߵ�
        await _synergySystem.OnTickBegin(new TurnContext(_board, attackTurn));

        /*
        //�׼� ����
        Dictionary<IUnit, UnitActionTypes> actionDic = new Dictionary<IUnit, UnitActionTypes>();
        List<IUnit> allUnit = new List<IUnit>();
        allUnit.AddRange(playerUnits);
        allUnit.AddRange(enemyUnits);

        
        foreach (IUnit unit in allUnit) {
            BaseUnit baseUnit = unit as BaseUnit;

            if (baseUnit.IsMovable(turnContext)) {
                actionDic.Add(unit, UnitActionTypes.Move);
            }
            else if (baseUnit.IsAttackable(turnContext)) {
                actionDic.Add(unit, UnitActionTypes.Attack);
            }
            else {
                actionDic.Add(unit, UnitActionTypes.None);
            }
        }
        */

        /*
        //�׼� ����
        foreach (IUnit unit in actionDic.Keys) {
            UnitActionTypes action = actionDic[unit];

            BaseUnit baseUnit = unit as BaseUnit;

            //���̶���Ʈ�ϰ� ������
            baseUnit.Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            switch (action) {
                case UnitActionTypes.Move:
                    baseUnit.Move(turnContext);
                    break;
                case UnitActionTypes.Attack:
                    baseUnit.AttackAction(turnContext);
                    break;
                case UnitActionTypes.None:
                    break;
            }

            baseUnit.Unhighlight();
        }
        */

        List<IUnit> attackTurnUnits = attackTurn == CharacterTypes.Player ? playerUnits : enemyUnits;
        List<IUnit> defenceTurnUnits = attackTurn == CharacterTypes.Player ? enemyUnits : playerUnits;

        //��� �� ���� �׼�
        foreach (IUnit unit in defenceTurnUnits) {
            if (unit.CurrentHP <= 0) continue;
            BaseUnit baseUnit = unit as BaseUnit;

            //���̶���Ʈ�ϰ� ������
            baseUnit.Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            //�̵��� �� �ִٸ�, �̵���Ű�� ���� ��������
            if (baseUnit.IsMovable(turnContext)) {
                baseUnit.Move(turnContext);
            }
            //�̵��� �� ���ٸ�, ������ �õ�
            else if (baseUnit.IsAttackable(turnContext)) {
                baseUnit.AttackAction(turnContext);
            }

            //���̶���Ʈ ����
            (unit as BaseUnit).Unhighlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
        }

        ProcessDeath(ref playerUnits, ref enemyUnits);

        //���� �� ���� �׼�
        foreach (IUnit unit in attackTurnUnits) {
            if (unit.CurrentHP <= 0) continue;
            BaseUnit baseUnit = unit as BaseUnit;

            //���̶���Ʈ�ϰ� ������
            baseUnit.Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            //�̵��� �� �ִٸ�, �̵���Ű�� ���� ��������
            if (baseUnit.IsMovable(turnContext)) {
                baseUnit.Move(turnContext);
            }
            //�̵��� �� ���ٸ�, ������ �õ�
            else if (baseUnit.IsAttackable(turnContext)) {
                baseUnit.AttackAction(turnContext);
            }

            //���̶���Ʈ ����
            (unit as BaseUnit).Unhighlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
        }

        ProcessDeath(ref playerUnits, ref enemyUnits);

        //�� ���� ������ ������ �ִٸ� �����ϰ� ������ ������
        foreach (var unit in attackTurnUnits) {
            int endCol = attackTurn == CharacterTypes.Player ? _board.Column - 1 : 0;
            if (unit.CurrentCell.position.col == endCol) {
                LifeDamage(attackTurn.Opponent(), unit.Attack);
                (unit as BaseUnit).Die();
            }
        }

        ProcessDeath(ref playerUnits, ref enemyUnits);

        await UniTask.Delay(TimeSpan.FromSeconds(delayPerTick));
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

    private bool CheckAttackable(IUnit unit) {
        IUnit targetUnit = GetAttackTarget(unit);
        if (targetUnit != null)
            return true;
        else
            return false;
    }
    //private void UnitAttack(IUnit unit) {
    //    IUnit targetUnit = GetAttackTarget(unit);
    //    targetUnit.TakeDamage(turnContext, unit.Attack);
    //}

    //�����Ÿ� ���� ������ �� �ִ� ������ ������ �� ������ ��ȯ, ������ null
    private IUnit GetAttackTarget(IUnit unit) {
        int range = unit.Range;
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        Cell currentCell = unit.CurrentCell;
        int originCol = currentCell.position.col;
        int originRow = currentCell.position.row;

        //����� ������ �켱���� ����
        for (int i = 1; i <= range; i++) {
            Cell targetCell = _board.GetCell(originCol + forwardOffset * i, originRow);
            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null && unit.Owner != targetUnit.Owner) {
                return targetUnit;
            }
        }

        return null;
    }

    private void MoveUnit(IUnit unit) {
        Cell currentCell = unit.CurrentCell;

        // �������� �� ĭ�� ��ġ ���
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //������ ���� ���� ������
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //���� �̵�
        currentCell.UnitOut();
        forwardCell.UnitIn(unit);
        unit.CurrentCell = forwardCell;

        //�� ���� �����ߴٸ� ������ ���̰� ������ ������
        int endCol = unit.Owner == CharacterTypes.Player ? _board.Column - 1 : 0;
        if (forwardCell.position.col == endCol) {
            LifeDamage(unit.Owner.Opponent(), unit.Attack);
            (unit as BaseUnit).Die();
        }
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
