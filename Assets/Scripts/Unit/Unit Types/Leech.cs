using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leech : BaseUnit
{
    public override void AttackAction(TurnContext turnContext) {
        base.AttackAction(turnContext);

        TakeHeal(turnContext, Attack);
    }
}
