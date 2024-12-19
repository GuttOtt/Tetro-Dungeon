using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EnumTypes;
using UnityEngine;


[CreateAssetMenu(fileName = "New Projectile Skill Config", menuName = "ScriptableObjects/Skill/ProjectileSkillConfig")]
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
    public float Speed { get => _speed; }
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
        _speed = config.Speed;
    }

    public override void Decorate(SkillConfig skillConfig) {
        if (skillConfig is ProjectileSkillConfig config) {
            _baseDamage += config.BaseDamage;
            _attackRatio += config.AttackRatio;
            _spellPowerRatio += config.SpellPowerRatio;
            _targetAmount += config.TargetAmount;

            if (config.ProjectileSprite != null)
                _projectileSprite = config.ProjectileSprite;
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
        FireProjectiles(activator, target, turnContext);
    }

    public override void RegisterToUnitEvents(BaseUnit unit) {
        foreach (UnitEventTypes unitEvent in UnitEvents) {
            switch (unitEvent) {
                case UnitEventTypes.OnAttacking:
                    unit.onAttacking += FireProjectiles;
                    break;
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked += FireProjectiles;
                    break;
            }
        }
    }

    public override void UnregisterToUnitEvents(BaseUnit unit)
    {foreach (UnitEventTypes unitEvent in UnitEvents) {
            switch (unitEvent) {
                case UnitEventTypes.OnAttacking:
                    unit.onAttacking -= FireProjectiles;
                    break;
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked -= FireProjectiles;
                    break;
            }
        }
    }

    private bool FireProjectiles(BaseUnit unit, BaseUnit mainTarget, TurnContext turnContext) {
        if (!CheckChance(1)) {
            return ShouldInterrupt;
        }

        BaseUnit presentTarget = mainTarget;

        for (int i = 0; i < TargetAmount; i++) {
            FireProjectile(unit, presentTarget, turnContext);
            presentTarget = turnContext.Board.GetClosestUnit(unit.CurrentCell, unit.Owner.Opponent(), 100) as BaseUnit;
        }

        return ShouldInterrupt;
    }

    private void FireProjectile(BaseUnit unit, BaseUnit target, TurnContext turnContext) {
        if (!CheckChance(1)){
            return;
        }

        GameObject go = new GameObject();
        go.transform.position = unit.transform.position;
        Projectile proj = go.AddComponent<Projectile>();
        SpriteRenderer spr = go.AddComponent<SpriteRenderer>();
        spr.sprite = _projectileSprite;

        //set sorting layer of spr
        int sortingLayerID = SortingLayer.NameToID("Projectile");
        spr.sortingLayerID = sortingLayerID;


        int damageAmount = Utils.CalculateDamageAmount(unit, _baseDamage, _attackRatio, _spellPowerRatio);
        Damage damage = new Damage(_damageType, damageAmount);

        proj.Init(turnContext, target, damage, unit.OnDamageDealt, _speed);

        return;
    }

}