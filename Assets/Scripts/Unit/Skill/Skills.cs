using Cysharp.Threading.Tasks;
using EnumTypes;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/// Config and Skill Class Bases
#region Models
public static class SkillFactory
{
    public static UnitSkill CreateSkill(SkillConfig config) {
        if (config is DamageSkillConfig damageConfig) {
            return new DamageSkill(damageConfig);  // DamageSkill 생성
        }
        // 다른 스킬 타입을 추가하려면 여기에 더 많은 조건을 추가할 수 있음.
        else {
            throw new System.ArgumentException("Unknown SkillConfig type");
        }
    }
}

public abstract class SkillConfig : ScriptableObject {
    [SerializeField] private string _skillName;
    [SerializeField] private string _skillDescription;
    [SerializeField] private float _skillChance;

    public string SkillName => _skillName;
    public string BaseCooldown => _skillDescription;
    public float SkillChance => _skillChance;

    // 스킬별 추가 설정은 하위 클래스에서 정의    
}

public abstract class UnitSkill
{
    protected string _skillName;
    protected SkillConfig config;
    private string _skillDescription;
    private float _skillChance;

    public string SkillName { get => _skillName; }

    public UnitSkill(SkillConfig config) {
        this.config = config;

        _skillName = config.SkillName;
        _skillChance = config.SkillChance;
    }
    public bool CheckChance(float chanceMultiplier) {
        float multipliedChance = _skillChance * (1 + chanceMultiplier);
        float r = Random.Range(0, 1f);
        return r <= multipliedChance;
    }

    public abstract void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target);

    public abstract void Decorate(SkillConfig config);
}
#endregion

#region Damage Skill

#endregion