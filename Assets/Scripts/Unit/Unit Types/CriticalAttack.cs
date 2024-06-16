using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalAttack : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        IUnit attackTarget = GetAttackTarget(turnContext.Board);

        int r = UnityEngine.Random.Range(0, 100);
        int damage = r < 25 ? Attack * 2 : Attack;

        attackTarget?.TakeDamage(turnContext,damage);
    }
}
