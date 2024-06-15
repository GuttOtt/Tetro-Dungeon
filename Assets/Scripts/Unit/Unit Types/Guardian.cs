using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : BaseUnit
{
    public override void TakeDamage(TurnContext turnContext, int damage) {
        if (damage > UnitTypeValue)
        {
            damage = UnitTypeValue;
        }

        base.TakeDamage(turnContext, damage);
    }
}
