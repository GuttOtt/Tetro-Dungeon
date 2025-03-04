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
    [SerializeField] protected bool _isEffectOnCells = false; //범위 내의 Cell을 parent로 이펙트를 생성하는지. false일 경우 Target Unit을 parent로 해서 이펙트 오브젝트를 생성함.

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
    [SerializeField] protected bool _isEffectOnCells = false; //범위 내의 Cell을 parent로 이펙트를 생성하는지. false일 경우 Target Unit을 parent로 해서 이펙트 오브젝트를 생성함.

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
            // Changes
            _damageType = damageSkillConfig.DamageType;
            _effectSprite = damageSkillConfig.EffectSprite;
            _isEffectOnCells = damageSkillConfig.IsEffectOnCells;

            // AoE: 교체 조건 - 현재 _aoe의 true 개수가 damageSkillConfig.AoE의 true 개수보다 작을 때만 교체
            int currentTrueCount = CountTrue(_aoe);
            int newTrueCount = CountTrue(damageSkillConfig.AoE);
            if (currentTrueCount < newTrueCount) {
                _aoe = damageSkillConfig.AoE;
            }

            // Damages
            _baseDamage += damageSkillConfig.BaseDamage;
            _attackRatio += damageSkillConfig.AttackRatio;
            _spellPowerRatio += damageSkillConfig.SpellPowerRatio;
        }
        else {
            Debug.LogWarning("Invalid config type for DamageSkill.");
        }
    }

    public override void Undecorate(SkillConfig config) {
        if (config is DamageSkillConfig damageSkillConfig) {
            // Changes
            DamageSkillConfig original = damageSkillConfig; // original config
            _damageType = original.DamageType;
            
            // AoE: 오직 현재 _aoe의 true 개수가 damageSkillConfig.AoE의 true 개수와 일치할 때만 original.AoE로 되돌림
            int currentTrueCount = CountTrue(_aoe);
            int newTrueCount = CountTrue(original.AoE);
            if (currentTrueCount == newTrueCount) {
                _aoe = original.AoE;
            }
            
            _effectSprite = original.EffectSprite;
            _isEffectOnCells = original.IsEffectOnCells;

            // Damages
            _baseDamage -= damageSkillConfig.BaseDamage;
            _attackRatio -= damageSkillConfig.AttackRatio;
            _spellPowerRatio -= damageSkillConfig.SpellPowerRatio;
        }
        else {
            Debug.LogWarning("Invalid config type for DamageSkill.");
        }
    }

    private int CountTrue(TArray<bool> tArray) {
        bool[,] array = tArray.GetArray<bool>();
        int count = 0;
        for (int i = 0; i < array.GetLength(0); i++) {
            for (int j = 0; j < array.GetLength(1); j++) {
                if (array[i, j])
                    count++;
            }
        }
        return count;
    }

    #region Activation
    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit mainTarget) {
        DealDamage(activator, mainTarget, turnContext);
    }
    #endregion

    private bool DealDamage(BaseUnit activator, BaseUnit mainTarget, TurnContext turnContext) {
        Board board = turnContext.Board;

        List<IUnit> targets = GetUnitsInAoE(board, activator, mainTarget, mainTarget.Owner);

        int damageAmount = Utils.CalculateDamageAmount(activator, _baseDamage, _attackRatio, _spellPowerRatio);
        Damage damage = new Damage(_damageType, damageAmount);

        foreach (BaseUnit target in targets) {
            if (target != null)
                target.TakeDamage(turnContext, damage);
            if (activator != null)
                activator.OnDamageDealt(turnContext, target, damage);
        }

        //Effect Sprite
        if (_effectSprite == null) {
            return ShouldInterrupt;
        }

        if (_isEffectOnCells) {
            List<Cell> targetCells = GetCellsInAoE(board, activator, mainTarget, CharacterTypes.None);
            CreateEffectSprite(targetCells).Forget();
        }
        else {
            List<BaseUnit> baseUnitList = targets.OfType<BaseUnit>().ToList();
            CreateEffectSprite(baseUnitList).Forget();
        }

        return ShouldInterrupt;
    }

    

    public override void RegisterToUnitEvents(BaseUnit unit) {
        unit.onAttacked += DealDamage;
    }

    public override void UnregisterToUnitEvents(BaseUnit unit) {
        unit.onAttacked -= DealDamage;
    }


    #region Getting units in AoE
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
        //스킬 시전 유닛과 타겟 유닛의 상대적 위치에 따라 AoE를 조정함

        int xActivator = activator.CurrentCell.position.col;
        int yActivator = activator.CurrentCell.position.row;
        int xTarget = mainTarget.CurrentCell.position.col;
        int yTarget = mainTarget.CurrentCell.position.row;

        int xOffset = xTarget - xActivator;
        int yOffset = yTarget - yActivator;

        bool[,] adjusted;


        //적이 우측에 있는 경우
        if (0 < xOffset) {
            adjusted = _aoe.GetArray<bool>();
        }
        //적이 좌측에 있는 경우
        else if (xOffset < 0) {
            adjusted = Utils.HorizontalFlip<bool>(_aoe);
        }
        //적이 아래에 있는 경우
        else if (yOffset < 0) {
            adjusted = Utils.RotateRight<bool>(_aoe);
        }
        //적이 위에 있는 경우
        else {
            adjusted = Utils.RotateLeft<bool>(_aoe);
        }

        return adjusted;
    }
    #endregion

    #region Presentating Effect Sprites
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

    /// <summary>
    /// 인자의 Cell을 Parent로 하는 EffectSprite를 생성
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