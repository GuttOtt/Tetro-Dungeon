using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dragon Synergy", menuName = "Synergy/Plant Synergy")]
public class PlantSynergy : BaseSynergy {
    public override void CoolTimeEffect(TurnContext turnContext, int synergyCount) {
        Debug.Log("�Ĺ� �ó��� ȿ�� �ߵ�");

        Board board = turnContext.Board;

        List<IUnit> playerUnits = board.GetUnits(CharacterTypes.Player);

        foreach (IUnit unit in playerUnits) {
            (unit as BaseUnit).TakeHeal(turnContext, synergyCount * (int) _synergyValue);

        }

        Debug.Log($"�Ʊ� ��ü���� �Ĺ� �ó����� ȿ���� {synergyCount * (int)_synergyValue}��ŭ ȸ��!");
    }
}
