using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Promotion : BaseUnit
{
    public override void Move(TurnContext turnContext) {
        base.Move(turnContext);
        base.ChangeAttack(UnitTypeValue);
        base.ChangeCurrentHP(UnitTypeValue);
    }
}
