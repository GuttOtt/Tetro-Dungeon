using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revenge : BaseUnit
{
    public override void Die(TurnContext turnContext) {
        base.Die(turnContext);
        var target = GetAttackTarget(turnContext.Board);
        if(target != null)
        {
            (target as BaseUnit).TakeDamage(turnContext, UnitTypeValue);
            Debug.Log($"{this.name}�� {(target as BaseUnit).name}���� ����!");
        }
    }
}
