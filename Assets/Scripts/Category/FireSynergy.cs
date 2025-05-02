using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire Synergy", menuName = "Synergy/Fire Synergy")]
public class FireSynergy : BaseSynergy {
    [SerializeField] private ExplosionStatusConfig explosionStatusConfig;
    [SerializeField] private Dictionary<int, int> explosionChancePercentPerSynergyCount = new();

    public override void OnBattleBegin(TurnContext turnContext, int synergyValue) {
        int maxValidCount = explosionChancePercentPerSynergyCount.Keys
            .Where(count => count <= synergyValue)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !explosionChancePercentPerSynergyCount.TryGetValue(maxValidCount, out int fearChancePercent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyValue})에 대한 공포 확률 증가 비율이 설정되지 않았습니다.");
            return;
        }

        // Player Unit의 OnAttacked 이벤트에 ApplyFear 메소드를 등록
        List<IUnit> playerUnits = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);
        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit != null) {
                baseUnit.onAttacked += (attacker, defender, turnContext) => ApplyExplosion(defender, fearChancePercent);
            }
        }
    }

    private bool ApplyExplosion(BaseUnit unit, int explosionChancePercent) {
        //chansePercent 확률로 공포 효과 적용
        if (Random.Range(0, 100) > explosionChancePercent) return false;

        ExplosionStatus explosionStatus = StatusFactory.CreateStatus(explosionStatusConfig) as ExplosionStatus;
        StatusApplicationContext context = new StatusApplicationContext(unit, null);
        unit.GrantStatus(explosionStatus, context);
        Debug.Log($"유닛 {unit.Name}에게 폭발 효과 적용!");

        return false;
    }
}
