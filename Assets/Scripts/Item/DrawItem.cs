using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Draw Item", menuName = "Item/Draw Item")]
public class DrawItem : Item {
    [SerializeField]
    private int _drawAmount;

    public override void OnSatisfiedEffect(TurnContext turnContext) {
        turnContext.CardSystem.DrawCard(_drawAmount);
    }
}
