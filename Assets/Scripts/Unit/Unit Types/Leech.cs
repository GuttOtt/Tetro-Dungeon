using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leech : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        base.AttackAction(turnContext);
        TakeHeal(turnContext, Attack * UnitTypeValue / 10);
        Debug.Log($"{base.name}�� ���� ȿ���� {Attack * UnitTypeValue / 100}��ŭ ȸ���ߴ�!");
    }
}
