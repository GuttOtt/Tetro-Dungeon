using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dragon Synergy", menuName = "Synergy/Dragon Synergy")]
public class DragonSynergy : BaseSynergy {

    public override void OnBattleBegin(TurnContext turnContext, int synergyCount) {
        Debug.Log("���� �ó��� ȿ�� �ߵ�");

        Board board = turnContext.Board;

        List<IUnit> enemyUnits = board.GetUnits(CharacterTypes.Enemy);

        int amount = synergyCount * (int)_synergyValue;

        foreach (IUnit unit in enemyUnits) {
            unit.TakeDamage(turnContext, amount);
        }

        Debug.Log($"���� �ó��� ȿ���� �� ��ü���� {amount} ��ŭ ������!");
    }
}
