using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class ProjectileSkillConfig : SkillConfig {
    [SerializeField] private Sprite _projectileSprite;
    [SerializeField] private int _baseDamage;
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private float _attackRatio, _spellPowerRatio, _speed;
    [SerializeField] private int _targetAmount;

    public Sprite ProjectileSprite { get { return _projectileSprite; }}
    public int BaseDamage { get { return _baseDamage;} }
    public float AttackRatio { get { return _attackRatio; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public int TargetAmount { get { return _targetAmount;} }
    public DamageTypes DamageType {get => _damageType;}
}


public class ProjectileSkill : UnitSkill {
    [SerializeField] private Sprite _projectileSprite;
    [SerializeField] private int _baseDamage;
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private float _attackRatio, _spellPowerRatio, _speed;
    [SerializeField] private int _targetAmount;
    private ProjectileSkillConfig _original;

    public Sprite ProjectileSprite { get { return _projectileSprite; } }
    public int BaseDamage { get { return _baseDamage; } }
    public float AttackRatio { get { return _attackRatio; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public int TargetAmount { get { return _targetAmount; } }
    public DamageTypes DamageType { get => _damageType; }

    public ProjectileSkill(ProjectileSkillConfig config) : base(config) {
        _original = config;
        _projectileSprite = config.ProjectileSprite;
        _baseDamage = config.BaseDamage;
        _damageType = config.DamageType;
        _attackRatio = config.AttackRatio;
        _spellPowerRatio = config.SpellPowerRatio;
        _targetAmount = config.TargetAmount;
    }

    public override void Decorate(SkillConfig skillConfig) {
        if (skillConfig is ProjectileSkillConfig config) {
            _projectileSprite = config.ProjectileSprite;
            _baseDamage += config.BaseDamage;
            _attackRatio += config.AttackRatio;
            _spellPowerRatio += config.SpellPowerRatio;
            _targetAmount += config.TargetAmount;
        }
        else {
            Debug.LogError("Invalid SkillConfig for ProjectileSkill.");
        }
    }

    public override void Undecorate(SkillConfig skillConfig) {
        if (skillConfig is ProjectileSkillConfig config) {
            _projectileSprite = _original.ProjectileSprite;
            _baseDamage -= config.BaseDamage;
            _attackRatio -= config.AttackRatio;
            _spellPowerRatio -= config.SpellPowerRatio;
            _targetAmount -= config.TargetAmount;
        }
        else {
            Debug.LogError("Invalid SkillConfig for ProjectileSkill.");
        }
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        FireProjectile(activator, target, turnContext);
    }

    public override void RegisterToUnitEvents(BaseUnit unit) {
        foreach (UnitEventTypes unitEvent in UnitEvents) {
            switch (unitEvent) {
                case UnitEventTypes.OnAttacking:
                    unit.onAttacking += FireProjectile;
                    break;
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked += FireProjectile;
                    break;
            }
        }
    }

    public override void UnregisterToUnitEvents(BaseUnit unit)
    {foreach (UnitEventTypes unitEvent in UnitEvents) {
            switch (unitEvent) {
                case UnitEventTypes.OnAttacking:
                    unit.onAttacking -= FireProjectile;
                    break;
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked -= FireProjectile;
                    break;
            }
        }
    }

    private bool FireProjectile(BaseUnit unit, BaseUnit target, TurnContext turnContext) {
        GameObject go = new GameObject();
        Projectile proj = go.AddComponent<Projectile>();
        SpriteRenderer spr = go.AddComponent<SpriteRenderer>();
        spr.sprite = _projectileSprite;

        int damage = (int) (_baseDamage + unit.Attack * _attackRatio + unit.SpellPower * _spellPowerRatio);

        proj.Init(turnContext, target, damage, _damageType);

        return ShouldInterrupt;
    }
}