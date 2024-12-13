using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkillConfig : SkillConfig {
    [SerializeField] private Sprite _projectileSprite;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _attackRatio, _spellPowerRatio, _speed;
    [SerializeField] private int _targetAmount;

    public Sprite ProjectileSprite { get { return _projectileSprite; }}
    public int BaseDamage { get { return _baseDamage;} }
    public float AttackRatio { get { return _attackRatio; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public int TargetAmount { get { return _targetAmount;} }
}


public class ProjectileSkill : UnitSkill {
    [SerializeField] private Sprite _projectileSprite;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _attackRatio, _spellPowerRatio, _speed;
    [SerializeField] private int _targetAmount;
    private ProjectileSkillConfig _original;

    public Sprite ProjectileSprite { get { return _projectileSprite; } }
    public int BaseDamage { get { return _baseDamage; } }
    public float AttackRatio { get { return _attackRatio; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public int TargetAmount { get { return _targetAmount; } }

    public ProjectileSkill(ProjectileSkillConfig config) : base(config) {
        _original = config;
        _projectileSprite = config.ProjectileSprite;
        _baseDamage = config.BaseDamage;
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
        throw new System.NotImplementedException();
    }

    private void FireProjectile() {

    }
}