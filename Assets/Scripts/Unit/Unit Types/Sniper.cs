using EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Sniper : BaseUnit
{

    public override void AttackAction(TurnContext turnContext) {
        IUnit farthest = turnContext.Board.GetFarthestUnit(CurrentCell, Owner.Opponent(), Range);

        if (farthest == null || farthest as BaseUnit == null)
            return;

        Action<BaseUnit> onHit = (target) => {
            target.TakeDamage(turnContext, Attack);
        };

        base.FireProjectile(farthest as BaseUnit, onHit);
    }
}
