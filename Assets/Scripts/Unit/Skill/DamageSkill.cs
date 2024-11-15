using Cysharp.Threading.Tasks;
using EnumTypes;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Skill", menuName = "ScriptableObjects/Skill/DamageSkill", order = 0)]
public class DamageSkill : ActiveSkill
{
    [SerializeField] protected DamageTypes damageType;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected float attackRatio;
    [SerializeField] protected float spellPowerRatio;
    [SerializeField] protected TArray<bool> aoe = new TArray<bool>();
    [SerializeField] protected Sprite effectSprite;
    [SerializeField] protected bool _isEffectOnCells = false; //���� ���� Cell�� parent�� ����Ʈ�� �����ϴ���. false�� ��� Target Unit�� parent�� �ؼ� ����Ʈ ������Ʈ�� ������.

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit mainTarget) {
        Board board = turnContext.Board;

        List<IUnit> targets = GetUnitsInAoE(board, activator, mainTarget, mainTarget.Owner);

        int damage = (int)(baseDamage + activator.Attack * attackRatio + activator.SpellPower * spellPowerRatio);
        
        foreach (IUnit target in targets) {
            Debug.Log($"{activator.Name}�� {(target as BaseUnit).Name}���� {name}���� {damage}��ŭ�� ������");
            target?.TakeDamage(turnContext, damage, damageType);
        }

        //Effect
        if (effectSprite == null) {
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
            spr.sprite = effectSprite;
            spr.sortingLayerName = "Effect";
            spr.color = new Color(1, 1, 1, 0.5f);

            effectSprites.Add(effectObject);
        }

        await UniTask.WaitForSeconds(0.6f);

        foreach (GameObject effectObject in effectSprites) {
            Destroy(effectObject);
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
            spr.sprite = effectSprite;
            spr.sortingLayerName = "Effect";
            spr.color = new Color(1, 1, 1, 0.5f);

            effectSprites.Add(effectObject);
        }

        await UniTask.WaitForSeconds(0.6f);

        foreach (GameObject effectObject in effectSprites) {
            Destroy(effectObject);
        }
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
            adjusted = aoe.GetArray<bool>();
        }
        //���� ������ �ִ� ���
        else if (xOffset < 0) {
            adjusted = Utils.HorizontalFlip<bool>(aoe);
        }
        //���� �Ʒ��� �ִ� ���
        else if (yOffset < 0) {
            adjusted = Utils.RotateRight<bool>(aoe);
        }
        //���� ���� �ִ� ���
        else {
            adjusted = Utils.RotateLeft<bool>(aoe);
        }

        return adjusted;
    }
}