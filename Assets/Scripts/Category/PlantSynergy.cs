using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dragon Synergy", menuName = "Synergy/Plant Synergy")]
public class PlantSynergy : BaseSynergy {
    public override void CoolTimeEffect(TurnContext turnContext, int synergyCount) {
        Debug.Log("식물 시너지 효과 발동");

        Board board = turnContext.Board;

        List<IUnit> playerUnits = board.GetUnits(CharacterTypes.Player);

        foreach (IUnit unit in playerUnits) {
            (unit as BaseUnit).TakeHeal(turnContext, synergyCount * (int) _synergyValue);

        }

        Debug.Log($"아군 전체에게 식물 시너지의 효과로 {synergyCount * (int)_synergyValue}만큼 회복!");
    }
}
