using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontHealer : BaseUnit
{
    public override bool IsAttackable(TurnContext turnContext) {
        Board board = turnContext.Board;

        IUnit targetUnit = board.GetClosestUnit(CurrentCell, Owner, Range);

        if (targetUnit != null && targetUnit.CurrentHP < targetUnit.MaxHP) {
            return true;
        }
        else {
            targetUnit = board.GetClosestUnit(CurrentCell, Owner.Opponent(), Range);

            if (targetUnit != null) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public override void AttackAction(TurnContext turnContext) {
        Board board = turnContext.Board;

        IUnit targetUnit = board.GetClosestUnit(CurrentCell, Owner, Range);

        if (targetUnit != null && targetUnit.CurrentHP < targetUnit.MaxHP) {
            targetUnit.TakeHeal(turnContext, Attack);
        }
        else {
            targetUnit = board.GetClosestUnit(CurrentCell, Owner.Opponent(), Range);

            if (targetUnit != null) {
                targetUnit.TakeDamage(turnContext, Attack);
            }
            else {
                Debug.LogError("Healer.IsAttackable returns true but it actually can't attack.");
            }
        }
    }
}
