using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New GrantStatus Skill Config", menuName = "ScriptableObjects/Skill/GrantStatusSkillConfig")]
public class GrantStatusSkillConfig : SkillConfig {
    [SerializeField] private StatusConfig _statusConfig;

    public StatusConfig StatusConfig { get => _statusConfig; }
}

public class GrantStatusSkill : UnitSkill {
    [SerializeField] private StatusConfig _statusConfig;
    private StatusConfig _originalStatusConfig;

    public GrantStatusSkill(GrantStatusSkillConfig config) : base(config) {
        _originalStatusConfig = config.StatusConfig;
        _statusConfig = config.StatusConfig;
    }

    public override void Decorate(SkillConfig config)
    {
        if (config is GrantStatusSkillConfig grantStatusConfig) {
            _statusConfig = grantStatusConfig.StatusConfig;
        }
        else {
            Debug.LogWarning("Invalid config type for GrantStatusSkill.");
        }
    }

    public override void Undecorate(SkillConfig config)
    {
        if (config is GrantStatusSkillConfig grantStatusConfig) {
            _statusConfig = _originalStatusConfig;
        }
        else {
            Debug.LogWarning("Invalid config type for GrantStatusSkill.");
        }
    }

    public override void RegisterToUnitEvents(BaseUnit unit) {
        foreach(UnitEventTypes unitEvent in UnitEvents) {
            switch (unitEvent) {
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked += GrantStatus;
                    break;
            }
        }
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        if (target != null) {
            GrantStatus(target);
        }
    }

    private bool GrantStatus(BaseUnit activator, BaseUnit target, TurnContext turncontext) {
        Status status = StatusFactory.CreateStatus(_statusConfig);
        target.GrantStatus(status);
        return ShouldInterrupt;
    }

    private void GrantStatus(BaseUnit targetUnit) {
        Status status = StatusFactory.CreateStatus(_statusConfig);
        targetUnit.GrantStatus(status);
    }
}
