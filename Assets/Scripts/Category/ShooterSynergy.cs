using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Shooter Synergy", menuName = "Synergy/Shooter Synergy")]
public class ShooterSynergy : BaseSynergy {
    [SerializeField] private SerializedDictionary<int, int> _doubleAttackChancePercentPerSynergyCount = new();
    [SerializeField] private DoubleAttackStatusConfig _doubleAttackStatusConfig;

    public override void OnBattleBegin(TurnContext turnContext, int synergyValue) {
        int maxValidCount = _doubleAttackChancePercentPerSynergyCount.Keys
            .Where(count => count <= synergyValue)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !_doubleAttackChancePercentPerSynergyCount.TryGetValue(maxValidCount, out int doubleAttackChancePercent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyValue})에 대한 추가 공격 확률 증가 비율이 설정되지 않았습니다.");
            return;
        }

        List<IUnit> playerUnits = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);

        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit != null && baseUnit.SynergyDict.ContainsKey(_synergyType)) {
                DoubleAttackStatus doubleAttackBuffStatus = StatusFactory.CreateStatus(_doubleAttackStatusConfig) as DoubleAttackStatus;
                doubleAttackBuffStatus.SetDoubleAttackChance(doubleAttackChancePercent);
                StatusApplicationContext context = new StatusApplicationContext(baseUnit, null);
                baseUnit.GrantStatus(doubleAttackBuffStatus, context);
                Debug.Log($"유닛 {baseUnit.Name}에게 추가 공격 확률 {doubleAttackChancePercent}% 증가 버프 적용!");
            }
        }

        Debug.Log($"아군 슈터 전원에게 추가 공격 확률 {doubleAttackChancePercent}% 증가 버프 적용!");
    }
}
