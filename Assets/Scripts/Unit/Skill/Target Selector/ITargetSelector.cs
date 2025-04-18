using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Extensions;
using UnityEngine;

public interface ITargetSelector
{
    List<BaseUnit> SelectTargets(BaseUnit activator, BaseUnit mainTarget);
}

public class TargetSelectorFactory
{
    public static ITargetSelector CreateTargetSelector(TargetSelectorConfig config)
    {
        switch (config.TargetType)
        {
            case TargetTypes.MainTarget:
                return new MainTargetSelector(config);
            case TargetTypes.AoE:
                return new AoETargetSelector(config);
            case TargetTypes.Activator:
                return new ActivatorSelector(config);
            default:
                throw new ArgumentException("Invalid target type");
        }
    }
}

[Serializable]
public class TargetSelectorConfig
{
    [SerializeField] private TargetTypes targetType;
    [SerializeField] private TArray<bool> aoe;
    [SerializeField] private bool selectAlly = false;

    public TargetTypes TargetType => targetType;
    public TArray<bool> Aoe => aoe;
    public bool SelectAlly => selectAlly;
}

public class BaseTargetSelector: ITargetSelector {
    private TargetTypes targetType;
    protected TArray<bool> aoe;
    protected bool selectAlly = false;

    public BaseTargetSelector(TargetSelectorConfig config) {
        targetType = config.TargetType;
        aoe = config.Aoe;
        selectAlly = config.SelectAlly;
    }

    public virtual List<BaseUnit> SelectTargets(BaseUnit activator, BaseUnit mainTarget) {
        // Implement logic to select targets based on the targetType and aoe
        // This is just a placeholder for the actual implementation
        return new List<BaseUnit> { mainTarget };
    }
}

public class MainTargetSelector : BaseTargetSelector
{
    public MainTargetSelector(TargetSelectorConfig config) : base(config) {
        // Initialize any specific properties for single target selection if needed
    }

    public override List<BaseUnit> SelectTargets(BaseUnit activator, BaseUnit mainTarget)
    {
        return new List<BaseUnit> { mainTarget };
    }
}

public class AoETargetSelector : BaseTargetSelector
{
    public AoETargetSelector(TargetSelectorConfig config) : base(config) {
        // Initialize any specific properties for AoE target selection if needed
    }

    public override List<BaseUnit> SelectTargets(BaseUnit activator, BaseUnit mainTarget)
    {
        TurnContext turnContext = TurnContextGenerator.Instance.GenerateTurnContext();
        Board board = turnContext.Board;

        CharacterTypes characterType = selectAlly ? activator.Owner : activator.Owner.Opponent();
        List<BaseUnit> targetUnits = GetUnitsInAoE(board, activator, mainTarget, characterType);

        return targetUnits;
    }

    private List<BaseUnit> GetUnitsInAoE(Board board, BaseUnit activator, BaseUnit mainTarget, CharacterTypes characterType) {
        Cell targetCell = mainTarget.CurrentCell;
        int targetCol = targetCell.position.col;
        int targetRow = targetCell.position.row;

        bool[,] adjustedAoE = AdjustAoE(activator, mainTarget);

        int top = targetRow - adjustedAoE.GetLength(1) / 2;
        int left = targetCol - adjustedAoE.GetLength(0) / 2;

        List<IUnit> unitsInAoE = board.GetUnitsInArea(adjustedAoE, characterType, top, left);
        List<BaseUnit> baseUnitsInAoE = new List<BaseUnit>();

        foreach (IUnit unit in unitsInAoE) {
            if (unit is BaseUnit baseUnit) {
                baseUnitsInAoE.Add(baseUnit);
            }
        }

        return baseUnitsInAoE;
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
            adjusted = aoe.GetArray<bool>();
        }
        //적이 좌측에 있는 경우
        else if (xOffset < 0) {
            adjusted = Utils.HorizontalFlip<bool>(aoe);
        }
        //적이 아래에 있는 경우
        else if (yOffset < 0) {
            adjusted = Utils.RotateRight<bool>(aoe);
        }
        //적이 위에 있는 경우
        else {
            adjusted = Utils.RotateLeft<bool>(aoe);
        }

        return adjusted;
    }
}

public class ActivatorSelector : BaseTargetSelector
{
    public ActivatorSelector(TargetSelectorConfig config) : base(config) {
        // Initialize any specific properties for activator target selection if needed
    }

    public override List<BaseUnit> SelectTargets(BaseUnit activator, BaseUnit mainTarget)
    {
        return new List<BaseUnit> { activator };
    }
}