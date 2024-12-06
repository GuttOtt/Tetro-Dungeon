using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domination : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        var target = GetAttackTarget(turnContext.Board);
        if (target.CurrentHP < UnitTypeValue)
        {
            Debug.Log($"{base.name}ÀÌ {(target as BaseUnit).name} Á¦¾Ð !");
            //(target as BaseUnit).Die();
        }
        else 
        {
            base.AttackAction(turnContext);
        }
    }
}
