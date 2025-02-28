using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New TakeDamageStatus Config", menuName = "ScriptableObjects/Status/TakeDamageStatusConfig")]
public class TakeDamageStatusConfig : StatusConfig {
    [SerializeField] private DamageTypes damageType;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _originalDamageRatio;
    [SerializeField] private float _maxHpExecutionThreshold;
    [SerializeField] private float onDieActivatorAttackBuffRatio;

    public DamageTypes DamageType { get => damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float OriginalDamageRatio { get => _originalDamageRatio; }
    public float MaxHpExecutionThreshold { get => _maxHpExecutionThreshold; }
    public float OnDieActivatorAttackBuffRatio => onDieActivatorAttackBuffRatio;
}

public class TakeDamageStatus : Status {
    private DamageTypes _damageType;
    private int _baseDamage;
    private float _originalDamageRatio;
    float _maxHpExecutionThreshold; // 최대 체력 대비 처형 임계값
    private float onDieActivatorAttackBuffRatio;
    private BaseUnit activator;

    public DamageTypes DamageType { get => _damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float OriginalDamageRatio { get => _originalDamageRatio; } 
    public float MaxHpExecutionThreshold { get => _maxHpExecutionThreshold; }
    public float OnDieActivatorAttackBuffRatio => onDieActivatorAttackBuffRatio;

    public TakeDamageStatus(TakeDamageStatusConfig config) : base(config) {
        _damageType = config.DamageType;
        _baseDamage = config.BaseDamage;
        _originalDamageRatio = config.OriginalDamageRatio;
        _maxHpExecutionThreshold = config.MaxHpExecutionThreshold;
        onDieActivatorAttackBuffRatio = config.OnDieActivatorAttackBuffRatio;
    }

    public override void ApplyTo(StatusApplicationContext context) {
        BaseUnit unit = context.TargetUnit;
        foreach (UnitEventTypes eventType in UnitEvents) {
            Register(unit, eventType);
        }
        activator = context.ActivatorUnit;
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
        foreach (UnitEventTypes eventType in UnitEvents) {
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
        Debug.Log("TakeDamageStatus: " + damageAmount + " damage applied to " + unit.name);

        // 현재 체력 / 최대 체력 비율이 임계값 이하면 즉시 처형
        if (_maxHpExecutionThreshold > 0 && ((float)unit.CurrentHP / unit.MaxHP) <= _maxHpExecutionThreshold) {
            unit.Die(turnContext);
            Debug.Log("TakeDamageStatus: " + unit.name + " executed" + "hp: " + unit.CurrentHP + "/" + unit.MaxHP + " threshold: " + _maxHpExecutionThreshold);
        }

        if (unit.CurrentHP <= 0 && onDieActivatorAttackBuffRatio > 0 && activator != null) {
            int attackBuff = (int) (activator.Stat.Attack * onDieActivatorAttackBuffRatio);
            activator.ChangeAttack(attackBuff);
            Debug.Log("TakeDamageStatus: " + activator.Name + " attack buffed by " + attackBuff);
            
        }
    }
}