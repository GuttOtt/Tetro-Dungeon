using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New TakeDamageStatus Config", menuName = "ScriptableObjects/Status/TakeDamageStatusConfig")]
public class TakeDamageStatusConfig : StatusConfig {
    [SerializeField] private DamageTypes damageType;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _originalDamageRatio;

    public DamageTypes DamageType { get => damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float OriginalDamageRatio { get => _originalDamageRatio; }
}

public class TakeDamageStatus : Status {
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _originalDamageRatio;

    public DamageTypes DamageType { get => _damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float OriginalDamageRatio { get => _originalDamageRatio; }


    public TakeDamageStatus(TakeDamageStatusConfig config) : base(config) {
        _damageType = config.DamageType;
        _baseDamage = config.BaseDamage;
        _originalDamageRatio = config.OriginalDamageRatio;
    }

    public override void ApplyTo(BaseUnit unit) {
        foreach(UnitEventTypes eventType in UnitEvents) {
            Register(unit, eventType);
        }
    }

    private void Register(BaseUnit unit, UnitEventTypes eventType) {
        switch (eventType) {
            case UnitEventTypes.OnAttacked:
                unit.onAttacked += ApplyDamage;
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
                unit.onAttacked -= ApplyDamage;
                break;
            case UnitEventTypes.OnDamageTaken:
                unit.onDamageTaken -= ApplyDamage;
                break;
        }
    }

    //Overload for onAttacked event
    private bool ApplyDamage(BaseUnit attackingUnit, BaseUnit AttackedUnit, TurnContext turnContext) {
        ApplyDamage(turnContext, attackingUnit, Damage.zero);
        return false;
    }

    private void ApplyDamage(TurnContext turnContext, BaseUnit unit, Damage originalDamage) {
        int damageAmount = _baseDamage + (int) (originalDamage.GetSum() * _originalDamageRatio);
        Damage damage = new Damage(_damageType, damageAmount);
        unit.TakeDamage(turnContext, damage, false);
    }
}