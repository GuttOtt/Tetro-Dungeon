using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Guardian : BaseUnit
{
    public override void TakeDamage(TurnContext turnContext, int damage) {
        if (damage > UnitTypeValue)
        {
            damage = UnitTypeValue;
            Debug.Log($"{base.name}�� {UnitTypeValue} ��ŭ�� �������� �޴´� !");
        }

        base.TakeDamage(turnContext, damage);
    }
}
