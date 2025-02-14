using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New DealDamageStatus Config", menuName = "ScriptableObjects/Status/DealDamageStatusConfig")]
public class DealDamageStatusConfig : StatusConfig {
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _originalDamageRatio;
    [SerializeField] private float _spellPowerRatio;
    [SerializeField] private float _attackRatio;
    [SerializeField] private float maxHPHealRatio;

    public DamageTypes DamageType { get => _damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float OriginalDamageRatio { get => _originalDamageRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public float AttackRatio { get => _attackRatio; }
    public float MaxHPHealRatio => maxHPHealRatio;
}

public class DealDamageStatus : Status{
    private DamageTypes _damageType;
    private int _baseDamage;
    private float _originalDamageRatio;
    private float _spellPowerRatio;
    private float _attackRatio;
    private int _ratioDamageAmount = 0;
    private float maxHPHealRatio;

    public DamageTypes DamageType { get => _damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float OriginalDamageRatio { get => _originalDamageRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public float AttackRatio { get => _attackRatio; }

    public DealDamageStatus(DealDamageStatusConfig config) : base(config) {
        _damageType = config.DamageType;
        _baseDamage = config.BaseDamage;
        _originalDamageRatio = config.OriginalDamageRatio;
        _spellPowerRatio = config.SpellPowerRatio;
        _attackRatio = config.AttackRatio;
        this.maxHPHealRatio = config.MaxHPHealRatio;
    }

    public override void ApplyTo(StatusApplicationContext context) {
        Debug.Log("DealDamageStatus Apply");
        if (context.ActivatorUnit != null) {
            BaseUnit activator = context.ActivatorUnit;
            _ratioDamageAmount = Utils.CalculateDamageAmount(activator, _baseDamage, _attackRatio, _spellPowerRatio);
        }

        foreach(UnitEventTypes eventType in UnitEvents) {
            Register(context.TargetUnit, eventType);
        }
    }

    private void Register(BaseUnit unit, UnitEventTypes eventType) {
        switch (eventType) {
            case UnitEventTypes.OnAttacked:
                unit.onDamageDealt += ApplyDamage;
                break;
            case UnitEventTypes.OnDamageTaken:
                unit.onDamageTaken += ApplyDamage;
                break;
        }
    }

    public override void RemoveFrom(BaseUnit unit) {
        foreach(UnitEventTypes eventType in UnitEvents) {
            Unregister(unit, eventType);
        }
    }

    private void Unregister(BaseUnit unit, UnitEventTypes eventType) {
        switch (eventType) {
            case UnitEventTypes.OnAttacked:
                unit.onDamageDealt -= ApplyDamage;
                break;
            case UnitEventTypes.OnDamageTaken:
                unit.onDamageTaken -= ApplyDamage;
                break;
        }
    }

    private void ApplyDamage(TurnContext turnContext, BaseUnit attackingUnit, BaseUnit attackedUnit, Damage damage){
        ApplyDamage(turnContext, attackedUnit, damage);

        // Heal the attacking unit
        int healAmount = (int) (attackedUnit.MaxHP * maxHPHealRatio);
        attackedUnit.TakeHeal(turnContext, healAmount);
    }

    private void ApplyDamage(TurnContext turnContext, BaseUnit unit, Damage originalDamage) {
        int damageAmount = _baseDamage + (int) (originalDamage.GetSum() * _originalDamageRatio) + _ratioDamageAmount;
        Damage damage = new Damage(_damageType, damageAmount);
        unit.TakeDamage(turnContext, damage, false);
    }
}