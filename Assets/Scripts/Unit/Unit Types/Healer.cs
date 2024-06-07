using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : BaseUnit
{
    public override bool IsAttackable(TurnContext turnContext) {
        Board board = turnContext.Board;

        int forwardOffset = Owner == EnumTypes.CharacterTypes.Player ? 1 : -1;
        Cell forwardCell = board.GetCell(CurrentCell.position.col + forwardOffset, CurrentCell.position.row);

        if (forwardCell != null || forwardCell.Unit == null)
            return false;
        else
            return true;
    }

    public override void AttackAction(TurnContext turnContext) {
        Board board = turnContext.Board;

        int forwardOffset = Owner == EnumTypes.CharacterTypes.Player ? 1 : -1;
        Cell forwardCell = board.GetCell(CurrentCell.position.col + forwardOffset, CurrentCell.position.row);
        
        if (forwardCell != null || forwardCell.Unit == null)
            return;

        IUnit targetUnit = forwardCell.Unit;

        if (targetUnit.Owner == Owner) {
            (targetUnit as BaseUnit).TakeHeal(turnContext, Attack);
        }
        else {
            (targetUnit as BaseUnit).TakeDamage(turnContext, Attack);
        }
    }
}
