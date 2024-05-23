using Cysharp.Threading.Tasks;
using EnumTypes;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    private Board _board;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _board = _gameManager.GetSystem<Board>();
    }

    public async UniTask ComputeTick(CharacterTypes attackTurn) {
        List<IUnit> playerUnits = _board.PlayerUnits.ToList();
        List<IUnit> enemyUnits = _board.EnemyUnits.ToList();

        //낮은 row 순으로, 같은 row에서는 앞쪽(상대방 쪽) 유닛 순으로 정렬
        playerUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenByDescending(unit => unit.CurrentCell.position.col);
        enemyUnits.OrderBy(unit => unit.CurrentCell.position.row)
            .ThenBy(unit => unit.CurrentCell.position.col);

        foreach (var unit in playerUnits) {
            //이동할 수 있다면, 이동시키고 다음 유닛으로
            if (MoveUnit(unit, attackTurn)) continue;

            
        }
    }

    private bool MoveUnit(IUnit unit, CharacterTypes attackTurn) {
        if (unit.Owner != attackTurn) return false;

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

        return true;
    }

    private void UnitAttck(IUnit unit) {
        int range = unit.Range;
        int forwardOffset = unit.Owner == CharacterTypes.Player ? 1 : -1;


    }
}
