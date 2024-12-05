using Cysharp.Threading.Tasks;
using EnumTypes;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Skill Config", menuName = "ScriptableObjects/Skill/DamageSkillConfig")]
public class DamageSkillConfig : SkillConfig
{
    [SerializeField] protected DamageTypes _damageType;
    [SerializeField] protected int _baseDamage;
    [SerializeField] protected float _attackRatio;
    [SerializeField] protected float _spellPowerRatio;
    [SerializeField] protected TArray<bool> _aoe = new TArray<bool>();
    [SerializeField] protected Sprite _effectSprite;
    [SerializeField] protected bool _isEffectOnCells = false; //���� ���� Cell�� parent�� ����Ʈ�� �����ϴ���. false�� ��� Target Unit�� parent�� �ؼ� ����Ʈ ������Ʈ�� ������.

    public DamageTypes DamageType { get => _damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float AttackRatio { get => _attackRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public TArray<bool> AoE { get => _aoe; }
    public Sprite EffectSprite { get => _effectSprite; }
    public bool IsEffectOnCells { get => _isEffectOnCells; }
}

public class DamageSkill : UnitSkill
{
    [SerializeField] protected DamageTypes _damageType;
    [SerializeField] protected int _baseDamage;
    [SerializeField] protected float _attackRatio;
    [SerializeField] protected float _spellPowerRatio;
    [SerializeField] protected TArray<bool> _aoe = new TArray<bool>();
    [SerializeField] protected Sprite _effectSprite;
    [SerializeField] protected bool _isEffectOnCells = false; //���� ���� Cell�� parent�� ����Ʈ�� �����ϴ���. false�� ��� Target Unit�� parent�� �ؼ� ����Ʈ ������Ʈ�� ������.

    public DamageTypes DamageType { get => _damageType; }
    public int BaseDamage { get => _baseDamage; }
    public float AttackRatio { get => _attackRatio; }
    public float SpellPowerRatio { get => _spellPowerRatio; }
    public TArray<bool> AoE { get => _aoe; }
    public Sprite EffectSprite { get => _effectSprite; }
    public bool IsEffectOnCells { get => _isEffectOnCells; }

    public DamageSkill(DamageSkillConfig config) : base(config) {
        _damageType = config.DamageType;
        _baseDamage = config.BaseDamage;
        _attackRatio = config.AttackRatio;
        _spellPowerRatio = config.SpellPowerRatio;
        _aoe = config.AoE;
        _effectSprite = config.EffectSprite;
        _isEffectOnCells = config.IsEffectOnCells;
    }

    public override void Decorate(SkillConfig config) {
        if (config is DamageSkillConfig damageSkillConfig) {
            //Changes
            _damageType = damageSkillConfig.DamageType;
            _aoe = damageSkillConfig.AoE;
            _effectSprite = damageSkillConfig.EffectSprite;
            _isEffectOnCells = damageSkillConfig.IsEffectOnCells;

            //Damages
            _baseDamage += damageSkillConfig.BaseDamage;
            _attackRatio += damageSkillConfig.AttackRatio;
            _spellPowerRatio += damageSkillConfig.SpellPowerRatio;
        }
        else {
            Debug.LogWarning("Invalid config type for DamageSkill.");
        }
    }

    #region Activation
    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit mainTarget) {
        Board board = turnContext.Board;

        List<IUnit> targets = GetUnitsInAoE(board, activator, mainTarget, mainTarget.Owner);

        int damage = (int)(_baseDamage + activator.Attack * _attackRatio + activator.SpellPower * _spellPowerRatio);

        foreach (IUnit target in targets) {
            Debug.Log($"{activator.Name}�� {(target as BaseUnit).Name}���� {_skillName}���� {damage}��ŭ�� ������");
            target?.TakeDamage(turnContext, damage, _damageType);
        }

        //Effect
        if (_effectSprite == null) {
            return;
        }

        if (_isEffectOnCells) {
            List<Cell> targetCells = GetCellsInAoE(board, activator, mainTarget, CharacterTypes.None);
            CreateEffectSprite(targetCells).Forget();
        }
        else {
            List<BaseUnit> baseUnitList = targets.OfType<BaseUnit>().ToList();
            CreateEffectSprite(baseUnitList).Forget();
        }

    }

    private List<IUnit> GetUnitsInAoE(Board board, BaseUnit activator, BaseUnit target, CharacterTypes opponentType) {
        Cell targetCell = target.CurrentCell;
        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] adjustedAoE = AdjustAoE(activator, target);

        int top = targetRow - adjustedAoE.GetLength(1) / 2;
        int left = targetCol - adjustedAoE.GetLength(0) / 2;

        List<IUnit> unitsInAoE = board.GetUnitsInArea(adjustedAoE, target.Owner, top, left);

        return unitsInAoE;
    }

    private List<Cell> GetCellsInAoE(Board board, BaseUnit activator, BaseUnit target, CharacterTypes opponentType) {
        Cell targetCell = target.CurrentCell;
        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] adjustedAoE = AdjustAoE(activator, target);

        int top = targetRow - adjustedAoE.GetLength(1) / 2;
        int left = targetCol - adjustedAoE.GetLength(0) / 2;

        List<Cell> cellsInAoE = board.GetCellsInArea(adjustedAoE, top, left);

        return cellsInAoE;
    }

    private bool[,] AdjustAoE(BaseUnit activator, BaseUnit mainTarget) {
        //��ų ���� ���ְ� Ÿ�� ������ ����� ��ġ�� ���� AoE�� ������

        int xActivator = activator.CurrentCell.position.col;
        int yActivator = activator.CurrentCell.position.row;
        int xTarget = mainTarget.CurrentCell.position.col;
        int yTarget = mainTarget.CurrentCell.position.row;

        int xOffset = xTarget - xActivator;
        int yOffset = yTarget - yActivator;

        bool[,] adjusted;


        //���� ������ �ִ� ���
        if (0 < xOffset) {
            adjusted = _aoe.GetArray<bool>();
        }
        //���� ������ �ִ� ���
        else if (xOffset < 0) {
            adjusted = Utils.HorizontalFlip<bool>(_aoe);
        }
        //���� �Ʒ��� �ִ� ���
        else if (yOffset < 0) {
            adjusted = Utils.RotateRight<bool>(_aoe);
        }
        //���� ���� �ִ� ���
        else {
            adjusted = Utils.RotateLeft<bool>(_aoe);
        }

        return adjusted;
    }
    #endregion

    #region Presentating Effect Sprites
    /// <summary>
    /// ������ Unit���� Parent�� �ϴ� EffectSprite�� ����
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

    /// <summary>
    /// ������ Cell�� Parent�� �ϴ� EffectSprite�� ����
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    private async UniTaskVoid CreateEffectSprite(List<Cell> cells) {
        List<GameObject> effectSprites = new List<GameObject>();

        foreach (Cell cell in cells) {
            GameObject effectObject = new GameObject();
            effectObject.transform.parent = cell.transform;
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
    #endregion
}