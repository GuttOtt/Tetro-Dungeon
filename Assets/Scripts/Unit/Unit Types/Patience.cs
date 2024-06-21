using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Patience : BaseUnit
{
    public override void TakeDamage(TurnContext turnContext, int damage) {
        base.TakeDamage(turnContext, damage);
        base.ChangeCurrentHP(UnitTypeValue);
        Debug.Log($"{base.name}의 인내심 효과!");
    }
}
