using EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrateArcher : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        BaseUnit target = turnContext.Board.GetClosestUnit(CurrentCell, Owner.Opponent(), Range) as BaseUnit;

        if (target == null) return;

        Vector2 direction = target.transform.position - transform.position;
        direction = direction.normalized;

        Action<BaseUnit> onHit = (hitUnit) => {
            if (hitUnit.Owner == Owner) return;

            hitUnit.TakeDamage(turnContext, Attack);
        };

        Debug.Log("Fire");

        FireProjectile(direction, onHit, Range, 7, 100);
    }
}
