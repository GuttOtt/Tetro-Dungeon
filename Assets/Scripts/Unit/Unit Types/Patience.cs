using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Patience : BaseUnit
{
    public override void TakeDamage(TurnContext turnContext, int damage, DamageTypes damageType = DamageTypes.True) {
        base.TakeDamage(turnContext, damage, damageType);
        base.ChangeCurrentHP(UnitTypeValue);
        Debug.Log($"{base.name}의 인내심 효과!");
    }
}
