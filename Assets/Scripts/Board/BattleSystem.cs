using Cysharp.Threading.Tasks;
using EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    private Board _board;
    
    //��Ʋ ���� ���� ������
    [SerializeField]
    private float delayPerUnit = 0.5f;
    [SerializeField]
    private float delayPerTick = 1f;

    private bool _isProcessing = false;
    #endregion

    [SerializeField]
    List<BaseUnit> list = new List<BaseUnit>();

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
    }

    public async void StartBattle() {
        if (_isProcessing)
            return;

        Debug.Log("Battle Started");
        _isProcessing = true;
        await ComputeTick(CharacterTypes.Player);
        _isProcessing= false;
    }

    public async UniTask ComputeTick(CharacterTypes attackTurn) {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();
        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //���� row ������, ���� row������ ����(���� ��) ���� ������ ����
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();

        //�÷��̾� ���� �׼�
        foreach (var unit in playerUnits) {
            //���̶���Ʈ�ϰ� ������
            (unit as BaseUnit).Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            //�̵��� �� �ִٸ�, �̵���Ű�� ���� ��������
            if (CheckMovable(unit, attackTurn)) {
                MoveUnit(unit);
            }
            //�̵��� �� ���ٸ�, ������ �õ�
            else {
                UnitAttack(unit);
            }

            //���̶���Ʈ ����
            (unit as BaseUnit).Unhighlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
        }

        //�� ���� �׼�
        foreach (var unit in enemyUnits) {
            //���̶���Ʈ�ϰ� ������
            (unit as BaseUnit).Highlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));

            //�̵��� �� �ִٸ�, �̵���Ű�� ���� ��������
            if (CheckMovable(unit, attackTurn)) {
                MoveUnit(unit);
            }
            //�̵��� �� ���ٸ�, ������ �õ�
            else {
                UnitAttack(unit);
            }

            //���̶���Ʈ ����
            (unit as BaseUnit).Unhighlight();
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
        }


        //������ ���� ó��
        ProcessDeath(playerUnits);
        ProcessDeath(enemyUnits);

        await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
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

    private void MoveUnit(IUnit unit) {
        Cell currentCell = unit.CurrentCell;

        // �������� �� ĭ�� ��ġ ���
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //������ ���� ���� ������
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //���� �̵��ϰ� return true
        currentCell.UnitOut();
        forwardCell.UnitIn(unit);
        unit.CurrentCell = forwardCell;
    }

    private void UnitAttack(IUnit unit) {
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
                //�����Ÿ� ���� ������ ������ �����ϰ� ���� ����
                targetUnit.TakeDamage(unit.Attack);
                break;
            }
        }
    }

    private void ProcessDeath(List<IUnit> units) {
        foreach (IUnit unit in units) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit.CurrentHP <= 0) {
                Destroy(baseUnit.gameObject);
            }
        }
    }
}
