using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword Synergy", menuName = "Synergy/Sword Synergy")]
public class SwordSynergy : BaseSynergy {
    [SerializeField] private SerializedDictionary<int, int> _attackPercentPerSynergyCount = new();
    [SerializeField] private StatBuffStatusConfig _statBuffStatusConfig;

    public override void OnBattleBegin(TurnContext turnContext, int synergyCount) {
        // synergyCount 이하의 값 중 가장 큰 count의 비율 찾기
        int maxValidCount = _attackPercentPerSynergyCount.Keys
            .Where(count => count <= synergyCount)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !_attackPercentPerSynergyCount.TryGetValue(maxValidCount, out int attackPercent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyCount})에 대한 공격력 증가 비율이 설정되지 않았습니다.");
            return;
        }

        // 플레이어 유닛들 가져오기
        List<IUnit> playerUnits = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);
        Debug.Log("PlayerUnitCount: " + turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player).Count);
        
        Stat statPercentageToBuff = new Stat(0, attackPercent, 0, 0, 0, 0, 0);

        // 각 유닛에게 공격력 버프 적용
        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit != null) {
                StatBuffStatus statBuffStatus = StatusFactory.CreateStatus(_statBuffStatusConfig) as StatBuffStatus;
                statBuffStatus.SetPercentageStatBuff(statPercentageToBuff);
                StatusApplicationContext context = new StatusApplicationContext(baseUnit, null);
                baseUnit.GrantStatus(statBuffStatus, context);
                Debug.Log($"유닛 {baseUnit.Name}에게 공격력 {attackPercent}% 증가 버프 적용!");
            }
        }

        Debug.Log($"아군 전체에게 공격력 {attackPercent}% 증가 버프 적용!");
    }
}
