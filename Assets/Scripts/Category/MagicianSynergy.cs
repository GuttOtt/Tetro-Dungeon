using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Magician Synergy", menuName = "Synergy/Magician Synergy")]
public class MagicianSynergy : BaseSynergy {
    [SerializeField] private SerializedDictionary<int, int> _spellPowerPercentPerSynergyCount = new();
    [SerializeField] private StatBuffStatusConfig _statBuffStatusConfig;

    public override void OnBattleBegin(TurnContext turnContext, int synergyCount) {
        int maxValidCount = _spellPowerPercentPerSynergyCount.Keys
            .Where(count => count <= synergyCount)
            .DefaultIfEmpty(0)
            .Max();

        if (maxValidCount == 0 || !_spellPowerPercentPerSynergyCount.TryGetValue(maxValidCount, out int spellPowerPercent)) {
            Debug.LogWarning($"해당 시너지 카운트({synergyCount})에 대한 주문력 증가 비율이 설정되지 않았습니다.");
            return;
        }

        List<IUnit> playerUnits = turnContext.Board.GetUnits(EnumTypes.CharacterTypes.Player);
        
        Stat statPercentageToBuff = new Stat(0, 0, spellPowerPercent, 0, 0, 0, 0);

        foreach (IUnit unit in playerUnits) {
            BaseUnit baseUnit = unit as BaseUnit;
            if (baseUnit != null && baseUnit.SynergyDict.ContainsKey(_synergyType)) {
                StatBuffStatus statBuffStatus = StatusFactory.CreateStatus(_statBuffStatusConfig) as StatBuffStatus;
                statBuffStatus.SetPercentageStatBuff(statPercentageToBuff);
                StatusApplicationContext context = new StatusApplicationContext(baseUnit, null);
                baseUnit.GrantStatus(statBuffStatus, context);
                Debug.Log($"유닛 {baseUnit.Name}에게 주문력 {spellPowerPercent}% 증가 버프 적용!");
            }
        }

        Debug.Log($"아군 마법사 전원에게 주문력 {spellPowerPercent}% 증가 버프 적용!");
    }
}
