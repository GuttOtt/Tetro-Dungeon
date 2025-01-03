using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatBuffStatus Config", menuName = "ScriptableObjects/Status/StatBuffStatusConfig")]
public class StatBuffStatusConfig : StatusConfig {
    [SerializeField] private Stat _constStatBuff;
    [SerializeField] private Stat _percentStatBuff;

    public Stat ConstStatBuff { get => _constStatBuff; }
    public Stat PercentStatBuff { get => _percentStatBuff; }
}

public class StatBuffStatus : Status {
    private Stat _constStatBuff;
    private Stat _percentStatBuff;
    private Stat _buffedStat;

    public Stat ConstStatBuff { get => _constStatBuff; }
    public Stat PercentStatBuff { get => _percentStatBuff; }

    public StatBuffStatus(StatBuffStatusConfig config) : base(config) {
        _constStatBuff = config.ConstStatBuff;
        _percentStatBuff = config.PercentStatBuff;
    }

    public override void ApplyTo(StatusApplicationContext context) {
        BaseUnit unit = context.TargetUnit;
        Stat unitStat = unit.Stat;
        _buffedStat = unitStat.PercentageMultiply(PercentStatBuff) + _constStatBuff;
        unit.GainStat(_buffedStat);
    }

    public override void RemoveFrom(BaseUnit unit) {
        unit.GainStat(-_buffedStat);
    }

}
