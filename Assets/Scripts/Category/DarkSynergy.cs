using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dark Synergy", menuName = "Synergy/Dark Synergy")]
public class DarkSynergy : BaseSynergy {
    [SerializeField] private FearStatusConfig _fearStatusConfig;
    [SerializeField] private SerializedDictionary<int, int> _fearChancePercentPerSynergyCount = new();

    public override void OnBattleBegin(TurnContext turnContext, int synergyValue) {
        int maxValidCount = _fearChancePercentPerSynergyCount.Keys
            .Where(count => count <= synergyValue)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !_fearChancePercentPerSynergyCount.TryGetValue(maxValidCount, out int fearChancePercent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyValue})에 대한 공포 확률 증가 비율이 설정되지 않았습니다.");
            return;
        }

        // Player Unit의 OnAttacked 이벤트에 ApplyFear 메소드를 등록
        List<IUnit> playerUnits = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);
        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit != null) {
                baseUnit.onAttacked += (attacker, defender, turnContext) => ApplyFear(defender, fearChancePercent);
            }
        }
    }

    private bool ApplyFear(BaseUnit unit, int fearChancePercent) {
        //chansePercent 확률로 공포 효과 적용
        if (Random.Range(0, 100) > fearChancePercent) return false;

        FearStatus fearStatus = StatusFactory.CreateStatus(_fearStatusConfig) as FearStatus;
        StatusApplicationContext context = new StatusApplicationContext(unit, null);
        unit.GrantStatus(fearStatus, context);
        Debug.Log($"유닛 {unit.Name}에게 공포 효과 적용!");

        return false;
    }
}
