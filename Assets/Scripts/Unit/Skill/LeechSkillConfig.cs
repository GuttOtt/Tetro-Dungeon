using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Leech Skill Config", menuName = "ScriptableObjects/Skill/LeechSkillConfig")]
public class LeechSkillConfig : SkillConfig {
    [SerializeField]
    private float _attackLeechRatio, _spellLeechRatio;
    
    public float AttackLeechRatio { get => _attackLeechRatio; }
    public float SpellLeechRatio { get => _spellLeechRatio; }
}

[Serializable]
public class LeechSkill : UnitSkill {
    private float _attackLeechRatio, _spellLeechRatio;

    public float AttackLeechRatio {get => _attackLeechRatio;}
    public float SpellLeechRatio {get => _spellLeechRatio;}

    private Action<TurnContext, BaseUnit, BaseUnit, Damage> _leechHandler;

    public LeechSkill(LeechSkillConfig config) : base(config) {
        _attackLeechRatio = config.AttackLeechRatio;
        _spellLeechRatio = config.SpellLeechRatio;
    }

    public override void Decorate(SkillConfig config) {
        if (config is LeechSkillConfig leechSkillConfig) {
            _attackLeechRatio += leechSkillConfig.AttackLeechRatio;
            _spellLeechRatio += leechSkillConfig.SpellLeechRatio;
        }
        else {
            Debug.LogError("Invalid config type for LeechSkill.");
        }
    }

    public override void Undecorate(SkillConfig config) {
        if (config is LeechSkillConfig leechSkillConfig) {
            _attackLeechRatio -= leechSkillConfig.AttackLeechRatio;
            _spellLeechRatio -= leechSkillConfig.SpellLeechRatio;
        }
        else {
            Debug.LogError("Invalid config type for LeechSkill");
        }
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        Debug.LogError("This skill can not be used as Active Skill.");
    }

    public override void RegisterToUnitEvents(BaseUnit unit) {
        unit.onDamageDealt += Leech;
        Debug.Log("Leech 스킬 등록");
    }

    public override void UnregisterToUnitEvents(BaseUnit unit) {
        unit.onDamageDealt -= Leech;
    }

    private void Leech(TurnContext turnContext, BaseUnit unit, BaseUnit target, Damage damageDealt) {
        int leechAmount = (int)(damageDealt.GetDamage(EnumTypes.DamageTypes.Attack) * _attackLeechRatio
            + damageDealt.GetDamage(EnumTypes.DamageTypes.Spell) * _spellLeechRatio);

        unit.TakeHeal(turnContext, leechAmount);
        Debug.Log($"{unit.Name}이 Leech Skill 효과로 {leechAmount} 만큼 회복!");
    }
}