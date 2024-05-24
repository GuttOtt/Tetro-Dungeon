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
    [SerializeField]
    private float delayPerUnit = 0.5f;
    [SerializeField]
    private float delayPerTick = 1f;
    #endregion

    [SerializeField]
    List<BaseUnit> list = new List<BaseUnit>();

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
    }

    public async void StartBattle() {
        Debug.Log("Battle Started");
        await ComputeTick(CharacterTypes.Player);
    }

    public async UniTask ComputeTick(CharacterTypes attackTurn) {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();
        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //���� row ������, ���� row������ ����(���� ��) ���� ������ ����
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();

        foreach (var unit in playerUnits) {
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
            //�̵��� �� �ִٸ�, �̵���Ű�� ���� ��������
            if (MoveUnit(unit, attackTurn)) continue;
            //�̵��� �� ���ٸ�, ������ �õ�
            UnitAttack(unit);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
    }

    private bool MoveUnit(IUnit unit, CharacterTypes attackTurn) {
        if (unit.Owner != attackTurn) {
            Debug.Log("�� ������ �̵� ���� �ƴմϴ�");
            return false;
        }
        Cell currentCell = unit.CurrentCell;

        // �������� �� ĭ�� ��ġ ���
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //������ ���� ���� ������
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //forwardCell�� �̹� ������ �ְų�, forwardCell�� �������� �ʴ´ٸ� false
        if (forwardCell == null || forwardCell.Unit != null) return false;

        //���� �̵��ϰ� return true
        currentCell.UnitOut();
        forwardCell.UnitIn(unit);
        unit.CurrentCell = forwardCell;

        return true;
    }

    private void UnitAttack(IUnit unit) {
        int range = unit.Range;
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        Cell currentCell = unit.CurrentCell;
        int originCol = currentCell.position.col;
        int originRow = currentCell.position.row;

        //����� ������ �켱���� ����
        for (int attackCol = originCol + 1; attackCol <= originCol + range; attackCol++) {
            Cell targetCell = _board.GetCell(attackCol, originRow);
            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null) {
                //�����Ÿ� ���� ������ ������ �����ϰ� ���� ����
                targetUnit.TakeDamage(unit.Attack);
                break;
            }
        }
    }
}
