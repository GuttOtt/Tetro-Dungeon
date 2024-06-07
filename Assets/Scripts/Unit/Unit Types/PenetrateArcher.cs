using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrateArcher : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        Debug.Log("Panetrate");
        List<IUnit> targets = new List<IUnit>();
        Board board = turnContext.Board;
        int range = Range;

        int forwardOffset = Owner == CharacterTypes.Player ? 1 : -1;
        Cell currentCell = CurrentCell;
        int originCol = currentCell.position.col;
        int originRow = currentCell.position.row;

        //사정거리 내의 모든 적 유닛을 공격
        for (int i = 1; i <= range; i++) {
            Cell targetCell = board.GetCell(originCol + forwardOffset * i, originRow);

            if (targetCell == null) continue;

            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null && Owner != targetUnit.Owner) {
                targetUnit.TakeDamage(Attack);
            }
        }
    }
}
