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

    public string Name { get => _name; }
    public string Description { get => _description; }
    public List<UnitEventTypes> UnitEvents { get => _unitEvents; }
    public List<SkillConfig> Skills {get => _skills; }
}


public abstract class Status {
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private List<UnitEventTypes> _unitEvents;


    public string Name {get => _name;}
    public string Description {get => _description;}
    public List<UnitEventTypes> UnitEvents {get => _unitEvents;}


    public Status(StatusConfig config) {
        _name = config.Name;
        _description = config.Description;
        _unitEvents = config.UnitEvents;
    }

    public abstract void ApplyTo(BaseUnit unit);

    public abstract void RemoveFrom(BaseUnit unit);
}