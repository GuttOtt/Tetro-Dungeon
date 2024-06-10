using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        int range = Range;
        int forwardOffset = Owner == CharacterTypes.Player ? 1 : -1;
        Cell currentCell = CurrentCell;
        int originCol = currentCell.position.col;
        int originRow = currentCell.position.row;

        //같은 라인에서 가장 뒤에 있는 적 유닛을 공격
        for (int i = range; 0 < i; i--) {
            Debug.Log("Board : " + turnContext.Board);
            Debug.Log($"originCol: {originCol}, forwardOffset: {forwardOffset}, originRow: {originRow}, i: {i}");
            Debug.Log("targetCell: " + turnContext.Board.GetCell(originCol + forwardOffset * i, originRow));

            Cell targetCell = turnContext.Board.GetCell(originCol + forwardOffset * i, originRow);

            if (targetCell == null) {
                continue;
            }

            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null && Owner != targetUnit.Owner) {
                targetUnit.TakeDamage(Attack);
                return;
            }
        }
    }
}
