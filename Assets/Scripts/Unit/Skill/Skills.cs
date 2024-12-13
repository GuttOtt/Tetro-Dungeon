using Cysharp.Threading.Tasks;
using EnumTypes;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnumTypes;
using UnityEngine.UIElements;

/// Config and Skill Class Bases
#region Models
public static class SkillFactory
{
    public static UnitSkill CreateSkill(SkillConfig config) {
        if (config is DamageSkillConfig damageConfig) {
            return new DamageSkill(damageConfig);  // DamageSkill 생성
        }
        if (config is ReviveSkillConfig reviveConfig) {
            return new ReviveSkill(reviveConfig);
        }
        if (config is HealClosestSkillConfig skillConfig) {
            return new HealClosestSkill(skillConfig);
        }
        else {
            throw new System.ArgumentException($"Unknown SkillConfig type: {config.GetType()}");
        }
    }

    public static List<UnitSkill> CreateSkills(List<SkillConfig> configs) {
        List<UnitSkill> skills = new List<UnitSkill>();
        
        foreach (SkillConfig config in configs) {
            skills.Add(CreateSkill(config));
        }

        return skills;
    }
}

public abstract class SkillConfig : ScriptableObject {
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillDescription;
    [SerializeField] private float _skillChance;
    [SerializeField] private List<EnumTypes.UnitEventTypes> _unitEvents;
    [SerializeField] private bool _shouldInterrupt;

    public string SkillName => _skillName;
    public string SkillDescription => _skillDescription;
    public float SkillChance { get => _skillChance; }
    public List<EnumTypes.UnitEventTypes> UnitEvents { get => _unitEvents; }
    public bool ShouldInterrupt { get => _shouldInterrupt; }

    // 스킬별 추가 설정은 하위 클래스에서 정의    
}

public abstract class UnitSkill
{
    protected string _skillName;
    protected SkillConfig config;
    private string _skillDescription;
    private float _skillChance;
    [SerializeField] private List<UnitEventTypes> _unitEvents;
    [SerializeField] private bool _shouldInterrupt;

    public string SkillName { get => _skillName; }
    public string SkillDescription { get => _skillDescription; }
    public float SkillChance => _skillChance;
    public List<UnitEventTypes> UnitEvents { get => _unitEvents; }
    public bool ShouldInterrupt { get => _shouldInterrupt; }

    public UnitSkill(SkillConfig config) {
        this.config = config;

        _skillName = config.SkillName;
        _skillChance = config.SkillChance;
        _skillDescription = config.SkillDescription;
        _unitEvents = config.UnitEvents;
        _shouldInterrupt = config.ShouldInterrupt;
        
    }
    public bool CheckChance(float chanceMultiplier) {
        float multipliedChance = _skillChance * (1 + chanceMultiplier);
        float r = Random.Range(0, 1f);
        return r <= multipliedChance;
    }

    public virtual void RegisterToUnitEvents(BaseUnit unit) {
        //자식 클래스에서 세부 구현
    }

    public virtual void UnregisterToUnitEvents(BaseUnit unit) {
        //자식 클래스에서 세부 구현
    }

    public abstract void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null);

    public abstract void Decorate(SkillConfig config);
    public abstract void Undecorate(SkillConfig config);
}
#endregion
