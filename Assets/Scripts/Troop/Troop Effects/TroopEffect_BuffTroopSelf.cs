using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffTroopSelf", menuName = "TroopEffect/BuffTroopSelf")]
public class TroopEffect_BuffTroopSelf : TroopEffect {
    #region private members
    [SerializeField]
    private int _attack, _hp;
    #endregion

    public override void OnPlace(TurnContext turnContext, UnitBlock unitBlock) {
        foreach (BaseUnit unit in unitBlock.Units) {
            unit.ChangeAttack(_attack);
            unit.ChangeCurrentHP(_hp);
        }
    }
}
