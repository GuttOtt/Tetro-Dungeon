using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Array2DEditor;
using Cysharp.Threading.Tasks;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExplosionStatus Config", menuName = "ScriptableObjects/Status/ExplosionStatusConfig")]
public class ExplosionStatusConfig : StatusConfig
{
    [SerializeField] private int _maxCount;
    [SerializeField] Array2DBool _explosionArea;
    [SerializeField] private DamageTypes _damageType;
    [SerializeField] private int _damageAmount;
    [SerializeField] private Sprite _effectSprite;


    public int MaxCount { get => _maxCount; }
    public Array2DBool ExplosionArea { get => _explosionArea; }
    public DamageTypes DamageType { get => _damageType; }
    public int Damage { get => _damageAmount; }
    public Sprite EffectSprite { get => _effectSprite; }
}

public class ExplosionStatus : Status {
    private int _maxCount;
    private Array2DBool _explosionArea;
    private DamageTypes _damageType;
    private int _damageAmount;
    private Sprite _effectSprite;

    private int _currentCount = 0;

    public int MaxCount { get => _maxCount; }
    public DamageTypes DamageType { get => _damageType; }
    public int DamageAmount { get => _damageAmount; }

    public ExplosionStatus(ExplosionStatusConfig config) : base(config) {
        _maxCount = config.MaxCount;
        _explosionArea = config.ExplosionArea;
        _damageType = config.DamageType;
        _damageAmount = config.Damage;
        _effectSprite = config.EffectSprite;
    }


    public override void ApplyTo(StatusApplicationContext context) {
        BaseUnit unit = context.TargetUnit;

        if (unit.GetStatus(Name) != null) {
            ExplosionStatus status = unit.GetStatus(Name) as ExplosionStatus;
            status.CountUp(unit);
        }
        else {
            _currentCount = 1;
        }
    }

    public override void RemoveFrom(BaseUnit unit) {
        unit.RemoveStatus(this);
    }


    public void CountUp(BaseUnit unit) {
        _currentCount++;

        if (_maxCount <= _currentCount){
            Explode(unit);
            _currentCount = 0;
        }
    }

    private void Explode(BaseUnit unit) {
        TurnContext turnContext = GameObject.FindObjectOfType<GameManager>().CreateTurnContext();
        Board board = turnContext.Board;

        List<BaseUnit> targets = GetTarget(board, unit);
        Damage damage = new Damage(_damageType, _damageAmount);

        foreach (BaseUnit target in targets) {
            target.TakeDamage(turnContext, damage);
        }

        List<Cell> cells = GetCells(board, unit);
        if (_effectSprite != null) {
            CreateEffectSprite(cells).Forget();
        }
    }

    private List<BaseUnit> GetTarget(Board board, BaseUnit center) {
        Cell targetCell = center.CurrentCell;

        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] area = _explosionArea.GetCells();
        int top = targetRow - area.GetLength(1) / 2;
        int left = targetCol - area.GetLength(0) / 2;

        List<IUnit> iUnits = board.GetUnitsInArea(area, center.Owner, top, left);
        List<BaseUnit> targets = new List<BaseUnit>();

        foreach (IUnit iUnit in iUnits) {
            if (iUnit is BaseUnit unit){
                targets.Add(unit);
            }
        }

        return targets;
    }

    private List<Cell> GetCells(Board board, BaseUnit center) {
        Cell targetCell = center.CurrentCell;

        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] area = _explosionArea.GetCells();
        int top = targetRow - area.GetLength(1) / 2;
        int left = targetCol - area.GetLength(0) / 2;

        List<Cell> cells = board.GetCellsInArea(area, top, left);

        return cells;
    }

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
}
