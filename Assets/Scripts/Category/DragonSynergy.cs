using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DragonSynergy : BaseSynergy {

    public override void OnBattleBegin(TurnContext turnContext, int synergyValue) {
        Debug.Log("���� �ó��� ȿ�� �ߵ�");

        Board board = turnContext.Board;

        List<IUnit> enemyUnits = board.GetUnits(CharacterTypes.Enemy);

        foreach (IUnit unit in enemyUnits) {
            unit.TakeDamage(turnContext, synergyValue);
        }
    }
}
