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
            Debug.Log($"{this.name}은 {(target as BaseUnit).name}에게 복수!");
        }
    }
}
