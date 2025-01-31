using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Random Target Damage Skill Config", menuName = "ScriptableObjects/Skill/RandomTargetDamageSkillConfig")]
public class RandomTargetDamageSkillConfig : SkillConfig {
    [SerializeField] private int _targetAmount;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _attackRatio, _spellPowerRatio;
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private Sprite _effectSprite;

    public int TargetAmount { get => _targetAmount; }
    public int BaseDamage { get => _baseDamage; }
    public float AttackRatio { get => _attackRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public DamageTypes DamageType { get => _damageType; }
    public Sprite EffectSprite {get => _effectSprite;}
}

public class RandomTargetDamageSkill : UnitSkill {
    private int _targetAmount;
    private int _baseDamage;
    private float _attackRatio, _spellPowerRatio;
    private DamageTypes _damageType;
    private Sprite _effectSprite;


    public int TargetAmount { get => _targetAmount; }
    public int BaseDamage { get => _baseDamage; }
    public float AttackRatio { get => _attackRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public DamageTypes DamageType { get => _damageType; }
    public Sprite EffectSprite {get => _effectSprite;}


    public RandomTargetDamageSkill(RandomTargetDamageSkillConfig config) : base(config) {
        _targetAmount = config.TargetAmount;
        _baseDamage = config.BaseDamage;
        _attackRatio = config.AttackRatio;
        _spellPowerRatio = config.SpellPowerRatio;
        _effectSprite  = config.EffectSprite;
        _damageType = config.DamageType;
    }

    public override void Decorate(SkillConfig config) {
        if (config is RandomTargetDamageSkillConfig randomTargetDamageSkillConfig) {
            _targetAmount += randomTargetDamageSkillConfig.TargetAmount;
            _baseDamage += randomTargetDamageSkillConfig.BaseDamage;
            _attackRatio += randomTargetDamageSkillConfig.AttackRatio;
            _spellPowerRatio += randomTargetDamageSkillConfig.SpellPowerRatio;
        }
        else {
            Debug.LogWarning("Invalid config type for RandomTargetDamageSkill.");
        }
    }
    public override void Undecorate(SkillConfig config) {
        if (config is RandomTargetDamageSkillConfig randomTargetDamageSkillConfig) {
            _targetAmount -= randomTargetDamageSkillConfig.TargetAmount;
            _baseDamage -= randomTargetDamageSkillConfig.BaseDamage;
            _attackRatio -= randomTargetDamageSkillConfig.AttackRatio;
            _spellPowerRatio -= randomTargetDamageSkillConfig.SpellPowerRatio;
        }
        else {
            Debug.LogWarning("Invalid config type for RandomTargetDamageSkill.");
        }
    
    }
    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        DealDamage(turnContext, activator);
    }

    private void DealDamage(TurnContext turnContext, BaseUnit activator) {
        Board board = turnContext.Board;
        int damageAmount = Utils.CalculateDamageAmount(activator, _baseDamage, _attackRatio, _spellPowerRatio);
        Damage damage = new Damage(_damageType, damageAmount);

        List<BaseUnit> targets = GetTargets(board, activator);

        foreach (BaseUnit target in targets) {
            if (target != null)
                target.TakeDamage(turnContext, damage);
        }

        CreateEffectSprite(targets).Forget();

    }

    private List<BaseUnit> GetTargets(Board board, BaseUnit activator) {
        if (board.GetUnits(activator.Owner.Opponent()).Count == 0) {
            return new List<BaseUnit>();
        }

        List<BaseUnit> targets = new List<BaseUnit>();

        for (int i = 0; i < _targetAmount; i++) {
            BaseUnit target = GetTarget(board, activator);
            targets.Add(target);
        }

        return targets;
    }

    private BaseUnit GetTarget(Board board, BaseUnit activator) {
        List<IUnit> opponentUnits = board.GetUnits(activator.Owner.Opponent());

        if (opponentUnits.Count == 0) {
            return null;
        }

        //Pick Random unit from opponentUnits
        IUnit iUnit = opponentUnits[Random.Range(0, opponentUnits.Count)];
        return iUnit as BaseUnit;
    }

    /// <summary>
    /// 인자의 Unit들을 Parent로 하는 EffectSprite를 생성
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    private async UniTaskVoid CreateEffectSprite(List<BaseUnit> units) {
        List<GameObject> effectSprites = new List<GameObject>();

        foreach (BaseUnit unit in units) {
            GameObject effectObject = new GameObject();
            effectObject.transform.parent = unit.transform;
            effectObject.transform.localPosition = Vector3.back;

            SpriteRenderer spr = effectObject.AddComponent<SpriteRenderer>();
            spr.sprite = _effectSprite;
            spr.sortingLayerName = "Effect";
            spr.color = new Color(1, 1, 1, 0.5f);

            effectSprites.Add(effectObject);
        }

        await UniTask.WaitForSeconds(0.6f);

        foreach (GameObject effectObject in effectSprites) {
            Object.Destroy(effectObject);
        }
    }
}
