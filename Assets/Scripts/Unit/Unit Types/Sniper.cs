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

        //���� ���ο��� ���� �ڿ� �ִ� �� ������ ����
        for (int i = range; 0 < i; i--) {
            Cell targetCell = turnContext.Board.GetCell(originCol + forwardOffset * i, originRow);

            if (targetCell == null) {
                continue;
            }

            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null && Owner != targetUnit.Owner) {
                targetUnit.TakeDamage(turnContext, Attack);
                return;
            }
        }
    }
}