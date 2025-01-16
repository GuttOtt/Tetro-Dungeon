using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Cysharp.Threading.Tasks;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New AoEDot Skill Config", menuName = "ScriptableObjects/Skill/AoEDotSkillConfig")]
public class AoEDotSkillConfig : SkillConfig {
    [SerializeField] private Array2DBool _area;
    [SerializeField] private int _baseDamage;
    [SerializeField] private float _attackRatio;
    [SerializeField] private float _spellPowerRatio;
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private float _duration;
    [SerializeField] private Sprite _effectSprite;
    public Array2DBool Area { get => _area; }
    public int BaseDamage { get => _baseDamage; }
    public float AttackRatio { get => _attackRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public DamageTypes DamageType { get => _damageType; }
    public float Duration { get => _duration; }
    public Sprite EffectSprite { get => _effectSprite; }
}

public class AoEDotSkill : UnitSkill {
    private int _baseDamage;
    private float _attackRatio;
    private float _spellPowerRatio;
    private DamageTypes _damageType;
    private Array2DBool _area;
    private float _duration;
    private Sprite _effectSprite;
    private Array2DBool _originalArea;


    public int BaseDamage { get { return _baseDamage; } }
    public float AttackRatio { get { return _attackRatio; } }
    public float SpellPowerRatio { get { return _spellPowerRatio; } }
    public DamageTypes DamageType { get { return _damageType; } }
    public Array2DBool Area {get => _area;}
    public float Duration {get => _duration;}

    
    public AoEDotSkill(AoEDotSkillConfig config) : base(config) {
        _baseDamage = config.BaseDamage;
        _attackRatio = config.AttackRatio;
        _spellPowerRatio = config.SpellPowerRatio;
        _damageType = config.DamageType;
        _area = config.Area;
        _duration = config.Duration;
        _effectSprite = config.EffectSprite;
    }

    public override void Decorate(SkillConfig config) {
        if (config is AoEDotSkillConfig aoeDotSkillConfig) {
            _baseDamage += aoeDotSkillConfig.BaseDamage;
            _attackRatio += aoeDotSkillConfig.AttackRatio;
            _spellPowerRatio += aoeDotSkillConfig.SpellPowerRatio;
            _duration += aoeDotSkillConfig.Duration;
            _area = aoeDotSkillConfig.Area;
        }
        else {
            Debug.LogWarning("Invalid config type for AoEDotSkill.");
        }
    }

    public override void Undecorate(SkillConfig config) {
        if (config is AoEDotSkillConfig aoeDotSkillConfig) {
            _baseDamage -= aoeDotSkillConfig.BaseDamage;
            _attackRatio -= aoeDotSkillConfig.AttackRatio;
            _spellPowerRatio -= aoeDotSkillConfig.SpellPowerRatio;
            _duration -= aoeDotSkillConfig.Duration;
            _area = _originalArea;
        }
        else {
            Debug.LogWarning("Invalid config type for AoEDotSkill.");
        }
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        CreateAoE(turnContext, activator, target.CurrentCell);
    }

    private async void CreateAoE(TurnContext turnContext, BaseUnit activator, Cell center) {
        int damageAmount = Utils.CalculateDamageAmount(activator, _baseDamage, _attackRatio, _spellPowerRatio);
        Damage damage = new Damage(_damageType, damageAmount);
        List<Cell> cells = GetCellsInArea(turnContext.Board, center);

        CreateEffectSprite(cells).Forget();

        //0.5초 간격으로 데미지를 입힘
        for (float i = 0; i < _duration; i+=0.5f) {
            ApplyDamage(cells, damage, activator.Owner.Opponent());
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }

    private void ApplyDamage(List<Cell> cells, Damage damage, CharacterTypes targetCharacterType) {
        foreach (Cell cell in cells) {
            BaseUnit unit = cell.Unit as BaseUnit;
            if (unit && unit.Owner == targetCharacterType)
                unit.TakeDamage(TurnContextGenerator.Instance.GenerateTurnContext(), damage, false);
        }
    }

    /// <summary>
    /// 인자의 Cell들을 Parent로 하는 EffectSprite를 생성
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    private async UniTaskVoid CreateEffectSprite(List<Cell> cells) {
        List<GameObject> effects = new List<GameObject>();

        foreach (Cell cell in cells) {
            GameObject effect = new GameObject("Effect");
            effect.transform.SetParent(cell.transform);
            effect.transform.localPosition = Vector3.zero;

            SpriteRenderer spriteRenderer = effect.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _effectSprite;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            spriteRenderer.sortingLayerName = "Effect";
            spriteRenderer.sortingOrder = 1;

            effects.Add(effect);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_duration));
        
        foreach (GameObject effect in effects) {
            GameObject.Destroy(effect);
        }
    }

    private List<Cell> GetCellsInArea(Board board, Cell center) {
        Cell targetCell = center;
        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] area = _area.GetCells();

        int top = targetRow - area.GetLength(0) / 2;
        int left = targetCol - area.GetLength(1) / 2;

        List<Cell> cellsInArea = board.GetCellsInArea(area, top, left);

        return cellsInArea;
    }
}