using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        base.AttackAction(turnContext);
        var target = GetAttackTarget(turnContext.Board);
        if(target == null)
        {
            ChangeAttack(UnitTypeValue);
            ChangeCurrentHP(UnitTypeValue);
            Debug.Log($"{this.name}의 파괴자 효과!");
        }
    }
}
