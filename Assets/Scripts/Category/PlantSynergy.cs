using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlantSynergy : BaseSynergy {
    public override void OnTickBegin(TurnContext turnContext, int synergyValue) {
        Debug.Log("�Ĺ� �ó��� ȿ�� �ߵ�");

        Board board = turnContext.Board;

        List<IUnit> playerUnits = board.GetUnits(CharacterTypes.Player);

        foreach (IUnit unit in playerUnits) {
            unit.TakeHeal(turnContext, synergyValue);

            Debug.Log($"{unit}�� �Ĺ� �ó����� ȿ���� {synergyValue}��ŭ ȸ���ߴ�!");
        }
    }
}
