using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dragon Synergy", menuName = "Synergy/Dragon Synergy")]
public class DragonSynergy : BaseSynergy {

    public override void OnBattleBegin(TurnContext turnContext, int synergyCount) {
        Debug.Log("용족 시너지 효과 발동");

        Board board = turnContext.Board;

        List<IUnit> enemyUnits = board.GetUnits(CharacterTypes.Enemy);

        int amount = synergyCount * (int)_synergyValue;

        foreach (IUnit unit in enemyUnits) {
            unit.TakeDamage(turnContext, amount);
        }

        Debug.Log($"용족 시너지 효과로 적 전체에게 {amount} 만큼 데미지!");
    }
}
