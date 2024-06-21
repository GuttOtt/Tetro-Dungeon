using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Draw", menuName = "TroopEffect/Draw")]
public class TroopEffect_Draw : TroopEffect {
    [SerializeField]
    private int _drawAmount = 0;

    public override void OnPlace(TurnContext turnContext, UnitBlock unitBlock) {
        turnContext.CardSystem.DrawCard(_drawAmount);
    }
}
