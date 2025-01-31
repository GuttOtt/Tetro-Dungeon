using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword Synergy", menuName = "Synergy/Sword Synergy")]
public class SwordSynergy : BaseSynergy {
    [SerializeField] private SerializedDictionary<int, float> _attackPercentPerSynergyCount = new();

    public override void OnBattleBegin(TurnContext turnContext, int synergyCount) {
        // synergyCount 이하의 값 중 가장 큰 count의 비율 찾기
        int maxValidCount = _attackPercentPerSynergyCount.Keys
            .Where(count => count <= synergyCount)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !_attackPercentPerSynergyCount.TryGetValue(maxValidCount, out float attackPercent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyCount})에 대한 공격력 증가 비율이 설정되지 않았습니다.");
            return;
        }

        // 플레이어 유닛들 가져오기
        List<IUnit> playerUnits = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);

        // 각 유닛에게 공격력 버프 적용
        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit != null) {
                int attackIncrease = (int)(baseUnit.Stat.Attack * (attackPercent / 100f));
                baseUnit.AddStatModifier(new StatModifier(StatTypes.Attack, attackIncrease));
            }
        }

        Debug.Log($"아군 전체에게 공격력 {attackPercent}% 증가 버프 적용!");
    }
}
