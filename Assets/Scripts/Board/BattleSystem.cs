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

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        playerUnits = playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();
        enemyUnits = enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col).ToList();

        foreach (var unit in playerUnits) {
            await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
            //이동할 수 있다면, 이동시키고 다음 유닛으로
            if (MoveUnit(unit, attackTurn)) continue;
            //이동할 수 없다면, 공격을 시도
            UnitAttack(unit);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(delayPerUnit));
    }

    private bool MoveUnit(IUnit unit, CharacterTypes attackTurn) {
        if (unit.Owner != attackTurn) {
            Debug.Log("이 유닛의 이동 턴이 아닙니다");
            return false;
        }
        Cell currentCell = unit.CurrentCell;

        // 전방으로 한 칸의 위치 계산
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //유닛의 앞쪽 셀을 가져옴
        Cell forwardCell = _board.GetCell(targetColumn, currentCell.position.row);

        //forwardCell에 이미 유닛이 있거나, forwardCell이 존재하지 않는다면 false
        if (forwardCell == null || forwardCell.Unit != null) return false;

        //유닛 이동하고 return true
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

        //가까운 유닛을 우선으로 공격
        for (int attackCol = originCol + 1; attackCol <= originCol + range; attackCol++) {
            Cell targetCell = _board.GetCell(attackCol, originRow);
            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null) {
                //사정거리 내에 유닛이 있으면 공격하고 루프 종료
                targetUnit.TakeDamage(unit.Attack);
                break;
            }
        }
    }
}
