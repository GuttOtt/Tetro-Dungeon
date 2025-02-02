using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New GrantStatus Skill Config", menuName = "ScriptableObjects/Skill/GrantStatusSkillConfig")]
public class GrantStatusSkillConfig : SkillConfig {
    [SerializeField] private StatusConfig _statusConfig;
    [SerializeField] private TargetTypes _targetType;


    public StatusConfig StatusConfig { get => _statusConfig; }
    public TargetTypes TargetType { get => _targetType; }
}

public class GrantStatusSkill : UnitSkill {
    private StatusConfig _statusConfig;
    private TargetTypes _targetType;
    private StatusConfig _originalStatusConfig;

    public StatusConfig StatusConfig { get => _statusConfig; }
    public TargetTypes TargetType { get => _targetType; }


    public GrantStatusSkill(GrantStatusSkillConfig config) : base(config) {
        _originalStatusConfig = config.StatusConfig;
        _statusConfig = config.StatusConfig;
        _targetType = config.TargetType;
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
        if (unit.GetStatus(_statusConfig.Name) != null)
            return;

        foreach(UnitEventTypes unitEvent in UnitEvents) {
            switch (unitEvent) {
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked += GrantStatus;
                    break;
                case UnitEventTypes.OnBattleStart:
                    unit.onBattleStart += GrantStatus;
                    break;
            }
        }
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        if (target != null) {
            StatusApplicationContext context = new StatusApplicationContext(target, activator);
            GrantStatus(activator, target, turnContext);
        }
    }

    private bool GrantStatus(BaseUnit activator, BaseUnit attackedTarget, TurnContext turncontext) {
        if (!CheckChance(1)){
            return false;
        }

        Status status = StatusFactory.CreateStatus(_statusConfig);

        List<BaseUnit> targets = GetTargets(activator, attackedTarget, turncontext.Board);
        if (targets == null || targets.Count == 0) {
            return ShouldInterrupt;
        }

        foreach (BaseUnit target in targets) {
            if (target == null) return ShouldInterrupt;

            StatusApplicationContext context = new StatusApplicationContext(target, activator);
            target.GrantStatus(status, context);
        }
        return ShouldInterrupt;
    }

    private void GrantStatus(BaseUnit activator, TurnContext turnContext) {
        GrantStatus(activator, null, turnContext);
    }

    private List<BaseUnit> GetTargets(BaseUnit activator,BaseUnit attackTarget, Board board) {
        List<BaseUnit> targets = new List<BaseUnit>();

        switch (TargetType) {
            case TargetTypes.AttackTarget:
                if (attackTarget == null) {
                    Debug.LogWarning("Target is null.");
                }
                targets.Add(attackTarget);
                break;
            case TargetTypes.ClosestAlly:
                BaseUnit closestAlly = board.GetClosestUnit(activator.CurrentCell, activator.Owner, 100) as BaseUnit;
                targets.Add(closestAlly);
                break;
            case TargetTypes.AllAllys:
                List<IUnit> units = board.GetUnits(activator.Owner);
                foreach (IUnit unit in units) {
                    targets.Add(unit as BaseUnit);
                }
                break;
        }

        return targets;
    }
}
