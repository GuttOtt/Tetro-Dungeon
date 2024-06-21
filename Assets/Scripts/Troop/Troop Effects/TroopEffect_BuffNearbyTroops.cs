using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffNearbyTroops", menuName = "TroopEffect/BuffNearbyTroops")]
public class TroopEffect_BuffNearbyTroops : TroopEffect {
    #region private members
    [SerializeField]
    private int _attack, _hp;
    #endregion

    public override void OnPlace(TurnContext turnContext, UnitBlock unitBlock) {
        List<UnitBlock> nearbyBlocks = turnContext.Board.GetNearbyBlocks(unitBlock);

        foreach (UnitBlock block in nearbyBlocks) {
            foreach (IUnit unit in block.Units) {
                (unit as BaseUnit).ChangeAttack(_attack);
                (unit as BaseUnit).ChangeCurrentHP(_hp);
            }
        }
    }
}
