using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public static class StatusFactory
{
    public static Status CreateStatus(StatusConfig config) {
        if (config is TakeDamageStatusConfig takeDamageStatusConfig) {
            return new TakeDamageStatus(takeDamageStatusConfig);
        }
        else if (config is ExplosionStatusConfig explosionStatusConfig) {
            return new ExplosionStatus(explosionStatusConfig);
        }
        else if (config is DealDamageStatusConfig dealDamageStatusConfig) {
            return new DealDamageStatus(dealDamageStatusConfig);
        }
        else if (config is StatBuffStatusConfig statBuffStatusConfig) {
            return new StatBuffStatus(statBuffStatusConfig);
        }
        else if (config is DoubleAttackStatusConfig doubleAttackStatusConfig) {
            return new DoubleAttackStatus(doubleAttackStatusConfig);
        }
        else {
            throw new System.ArgumentException($"Unknown SkillConfig type: {config.GetType()}");
        }
    }

    public static List<Status> CreateStatus(List<StatusConfig> configs) {
        List<Status> statuses = new List<Status>();

        foreach (StatusConfig config in configs) {
            statuses.Add(CreateStatus(config));
        }

        return statuses;
    }
}

public class StatusConfig: ScriptableObject {
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private List<UnitEventTypes> _unitEvents;
    [SerializeField] private List<SkillConfig> _skills;
    [SerializeField] private bool _isStackable;
    [SerializeField] private Sprite _iconSprite;

    public string Name { get => _name; }
    public string Description { get => _description; }
    public List<UnitEventTypes> UnitEvents { get => _unitEvents; }
    public List<SkillConfig> Skills {get => _skills; }
    public bool IsStackable { get => _isStackable; }
    public Sprite Sprite { get => _iconSprite; }
}


[Serializable]
public abstract class Status {
    private string _name;
    private string _description;
    private List<UnitEventTypes> _unitEvents;
    private bool _isStackable;
    private Sprite _iconSprite;

    public bool IsStackable{get=>_isStackable;}

    public string Name {get => _name;}
    public string Description {get => _description;}
    public List<UnitEventTypes> UnitEvents {get => _unitEvents;}
    public Sprite IconSprite {get => _iconSprite;}



    public Status(StatusConfig config) {
        _name = config.Name;
        _description = config.Description;
        _unitEvents = config.UnitEvents;
        _isStackable = config.IsStackable;
        _iconSprite = config.Sprite;
    }

    public abstract void ApplyTo(StatusApplicationContext context);

    public abstract void RemoveFrom(BaseUnit unit);
}

public class StatusApplicationContext {
    public BaseUnit TargetUnit {get; private set;}
    public BaseUnit ActivatorUnit {get; private set;}

    public StatusApplicationContext(BaseUnit target, BaseUnit activator) {
        TargetUnit = target;
        ActivatorUnit = activator;
    }
}