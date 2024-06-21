using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class AoEHealer : BaseUnit
{
    public override bool IsAttackable(TurnContext turnContext) {
        return true;
    }

    public override void AttackAction(TurnContext turnContext) {
        Board board = turnContext.Board;

        List<Cell> nearbyCells = board.GetNearbyCells(CurrentCell, false);
        
        foreach (Cell nearbyCell in nearbyCells) {
            if (nearbyCell.Unit != null && nearbyCell.Unit.Owner == Owner) {
                (nearbyCell.Unit as BaseUnit).TakeHeal(turnContext, Attack / 4);
            }
        }
    }
}
