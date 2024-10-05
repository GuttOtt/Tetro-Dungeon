using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Sadism : BaseUnit
{
    public override void TakeDamage(TurnContext turnContext, int damage, DamageTypes damageType = DamageTypes.True) {
        base.TakeDamage(turnContext, damage, damageType);
        base.ChangeAttack(UnitTypeValue);
        Debug.Log($"{base.name}의 가학성 효과!");
    }
}
