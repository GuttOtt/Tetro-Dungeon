using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leech : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        base.AttackAction(turnContext);
        TakeHeal(turnContext, Attack * UnitTypeValue / 10);
        Debug.Log($"{base.name}은 흡혈 효과로 {Attack * UnitTypeValue / 100}만큼 회복했다!");
    }
}
