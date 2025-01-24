using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Light Synergy", menuName = "Synergy/Light Synergy")]
public class LightSynergy : BaseSynergy {
    [SerializeField] private SerializedDictionary<int, float> _healPercentPerSynergyCount = new();

    public override void CoolTimeEffect(TurnContext turnContext, int synergyCount) {
        Debug.Log("빛 시너지 효과 발동");

        // synergyCount 이하의 값 중 가장 큰 count의 ratio 찾기
        int maxValidCount = _healPercentPerSynergyCount.Keys
            .Where(count => count <= synergyCount)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !_healPercentPerSynergyCount.TryGetValue(maxValidCount, out float healPerecent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyCount})에 대한 힐 비율이 설정되지 않았습니다.");
            return;
        }

        Board board = turnContext.Board;
        List<IUnit> playerUnits = board.GetUnits(EnumTypes.CharacterTypes.Player);

        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            int healAmount = (int)(baseUnit.MaxHP * (healPerecent / 100f));
            baseUnit.TakeHeal(turnContext, healAmount);
        }

        Debug.Log($"아군 전체에게 빛 시너지의 효과로 최대 체력의 {healPerecent}% 만큼 회복!");
    }
}