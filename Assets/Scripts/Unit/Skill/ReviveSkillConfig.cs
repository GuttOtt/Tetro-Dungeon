using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSkillConfig : SkillConfig {
    [SerializeField] private int _restoringHP;
    [SerializeField] private int _reviveCount;

    public int RestoringHP { get { return _restoringHP; } }
    public int ReviveCount { get { return _reviveCount; }}
}

public class ReviveSkill : UnitSkill {
    private int _restoringHP;
    private int _reviveCount;

    public int RestoringHP { get { return _restoringHP; } }
    public int ReviveCount { get { return _reviveCount; } }

    public ReviveSkill(ReviveSkillConfig config) : base(config) { 
        _restoringHP = config.RestoringHP;
        _reviveCount= config.ReviveCount;
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        
    }

    public override void RegisterToUnitEvents(BaseUnit unit) {
        base.RegisterToUnitEvents(unit);

        unit.onDying += Revive;
    }

    public override void UnregisterToUnitEvents(BaseUnit unit) {
        base.UnregisterToUnitEvents(unit);

        unit.onDying -= Revive;
    }

    private bool Revive(BaseUnit unit, TurnContext turnContext) {
        if (unit.CurrentHP <= 0 && 0 < _reviveCount) {
            //unit.TakeHeal(turnContext, _restoringHP)
            _reviveCount--;
            return true;
        }

        return false;
    }

    public override void Decorate(SkillConfig config) {
        if (config is ReviveSkillConfig reviveSkillConfig) {
            _restoringHP += reviveSkillConfig.RestoringHP;
            _reviveCount += reviveSkillConfig.ReviveCount;
        }
        else {
            Debug.LogWarning("Invalid config type for ReviveSkill");
        }
    }
}
