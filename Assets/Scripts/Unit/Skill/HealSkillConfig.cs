using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Closest Skill Config", menuName = "ScriptableObjects/Skill/HealSkillConfig")]
public class HealClosestSkillConfig : SkillConfig {
    [SerializeField] private int _baseHealAmount;
    [SerializeField] private float _spellPowerRatio;
    [SerializeField] private float _attackRatio;
    [SerializeField] private int _targetAmount;

    public int BaseHealAmount { get { return _baseHealAmount; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public float AttackRatio { get { return _attackRatio; } }
    public int TargetAmount { get { return _targetAmount;} }
}

public class HealClosestSkill : UnitSkill {
    [SerializeField] private int _baseHealAmount;
    [SerializeField] private float _spellPowerRatio;
    [SerializeField] private float _attackRatio;
    [SerializeField] private int _targetAmount;

    private Func<BaseUnit, BaseUnit, TurnContext, bool> _healHandler;

    public int BaseHealAmount { get { return _baseHealAmount; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public float AttackRatio { get { return _attackRatio; } }
    public int TargetAmount { get => _targetAmount; }

    public HealClosestSkill(HealClosestSkillConfig config) : base(config) { 
        _baseHealAmount = config.BaseHealAmount;
        _spellPowerRatio = config.SpellPowerRatio;
        _attackRatio = config.AttackRatio;
        _targetAmount = config.TargetAmount;
    }

    public override void RegisterToUnitEvents(BaseUnit unit) {
        _healHandler = (activator, target, turnContext) => {
            return Heal(activator, turnContext);
        };
        unit.onAttacked += _healHandler;
    }

    public override void UnregisterToUnitEvents(BaseUnit unit) {
        unit.onAttacked -= _healHandler;
    }

    public override void Decorate(SkillConfig config) {
        if (config is HealClosestSkillConfig healSkillConfig) {
            _baseHealAmount += healSkillConfig.BaseHealAmount;
            _spellPowerRatio += healSkillConfig.SpellPowerRatio;
            _attackRatio += healSkillConfig.AttackRatio;
            _targetAmount += healSkillConfig.TargetAmount;
        }
        else {
            Debug.LogWarning("Invalid config type for HealSkill.");
        }
    }
    public override void Undecorate(SkillConfig config) {
        if (config is HealClosestSkillConfig healSkillConfig) {
            _baseHealAmount -= healSkillConfig.BaseHealAmount;
            _spellPowerRatio -= healSkillConfig.SpellPowerRatio;
            _attackRatio -= healSkillConfig.AttackRatio;
            _targetAmount -= healSkillConfig.TargetAmount;
        }
        else {
            Debug.LogWarning("Invalid config type for HealSkill.");
        }
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        Heal(activator, turnContext);
    }

    private bool Heal(BaseUnit activator,TurnContext turnContext) {
        Debug.Log("힐 스킬 발동");
        int healAmount = (int) (_baseHealAmount + _attackRatio * activator.Attack
            + _spellPowerRatio * activator.SpellPower);

        List<BaseUnit> closestUnits = new List<BaseUnit>();

        //Getting closest Units
        BaseUnit indexUnit = activator;
        for (int i = 0; i < _targetAmount; i++) {
            BaseUnit closest = GetClosestUnit(indexUnit, turnContext.Board);
            closestUnits.Add(closest);
            indexUnit = closest;
        }

        foreach (BaseUnit unit in closestUnits) {
            if (unit == null)
                continue;

            unit.TakeHeal(turnContext, healAmount);
            Debug.Log($"{activator.Name}, id {activator.ID}가 {unit.Name}, id {unit.ID}를 {healAmount}만큼 회복!");
        }

        return ShouldInterrupt;
    }

    private BaseUnit GetClosestUnit(BaseUnit unit, Board board) {
        return board.GetClosestUnit(unit.CurrentCell, unit.Owner, 100) as BaseUnit;
    }

}
