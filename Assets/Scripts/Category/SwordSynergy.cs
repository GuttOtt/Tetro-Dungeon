using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword Synergy", menuName = "Synergy/Sword Synergy")]
public class SwordSynergy : BaseSynergy {
    

    public override void OnBattleBegin(TurnContext turnContext, int synergyValue) {
        //All player units get attack buff

    }
}
