using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Skill Config", menuName = "ScriptableObjects/Skill/MoveSkillConfig")]
public class MoveSkillConfig : SkillConfig
{
    [SerializeField] private TargetSelectorConfig targetSelectorConfig;
    [SerializeField] private MoveDirectionType moveDirectionType;
    [SerializeField] private int moveDistance;

    public MoveDirectionType MoveDirectionType => moveDirectionType;
    public int MoveDistance => moveDistance;
    public TargetSelectorConfig TargetSelectorConfig => targetSelectorConfig;
}

public class MoveSkill : UnitSkill
{
    private MoveDirectionType moveDirectionType;
    private int moveDistance;
    private ITargetSelector targetSelector;
    private Board board;

    public MoveSkill(MoveSkillConfig config) : base(config) {
        moveDirectionType = config.MoveDirectionType;
        moveDistance = config.MoveDistance;
        targetSelector = TargetSelectorFactory.CreateTargetSelector(config.TargetSelectorConfig);
        board = TurnContextGenerator.Instance.GenerateTurnContext().Board;
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) 
    {
        List<BaseUnit> targets = targetSelector.SelectTargets(activator, target);

        MoveTargets(targets, activator);
    }

    public override void Decorate(SkillConfig config) {
        // Implement decoration logic if needed
    }

    public override void Undecorate(SkillConfig config) {
        // Implement undecoration logic if needed
    }

    public override void RegisterToUnitEvents(BaseUnit unit)
    {
        foreach (UnitEventTypes unitEventType in UnitEvents) {
            switch (unitEventType) {
                case UnitEventTypes.OnBattleStart:
                    unit.onBattleStart += MoveTargets;
                    break;
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked += MoveTargets;
                    break;
            }
        }
    }

    public override void UnregisterToUnitEvents(BaseUnit unit) {
        foreach (UnitEventTypes unitEventType in UnitEvents) {
            switch (unitEventType) {
                case UnitEventTypes.OnBattleStart:
                    unit.onBattleStart -= MoveTargets;
                    break;
                case UnitEventTypes.OnAttacked:
                    unit.onAttacked -= MoveTargets;
                    break;
            }
        }
    }

    // Overloading for onBattleStart event
    private void MoveTargets(BaseUnit activator, TurnContext turnContext) {
        List<BaseUnit> targets = targetSelector.SelectTargets(activator, null);

        MoveTargets(targets, activator);
    }

    // Overloading for onAttacked event
    private bool MoveTargets(BaseUnit activator, BaseUnit target, TurnContext turnContext) {
        Debug.Log("MoveTargets called from onAttacked event");
        List<BaseUnit> targets = targetSelector.SelectTargets(activator, target);

        MoveTargets(targets, activator);

        return ShouldInterrupt;
    }

    private void MoveTargets(List<BaseUnit> targets, BaseUnit activator) {
        if (!CheckChance(1)) {
            Debug.Log("MoveTargets: Chance check failed.");
            return;
        }

        foreach (var target in targets) {
            switch (moveDirectionType) {
                case MoveDirectionType.Forward:
                    MoveVertical(target, target.Owner == CharacterTypes.Player ? 1 : -1);
                    break;
                case MoveDirectionType.Backward:
                    MoveVertical(target, target.Owner == CharacterTypes.Player ? -1 : 1);
                    break;
                case MoveDirectionType.Backline:
                    MoveBackline(target);
                    break;
                default:
                    Debug.LogWarning("Invalid move direction type.");
                    break;
            }
        }
    }

    private void MoveVertical(BaseUnit target, int moveDirection){
        Debug.Log($"MoveVertical called. target: {target.Name}, moveDirection: {moveDirection}");
        Cell currentCell = target.CurrentCell;
        int currentCol = currentCell.position.col;
        int currentRow = currentCell.position.row;
        int moveTargetCol = currentCol + moveDirection * moveDistance;
        Cell moveCell = currentCell;
        
        for (int col = currentCol + moveDirection; moveDirection == -1 ? moveTargetCol <= col : col <= moveTargetCol; col += moveDirection) {
            Cell cell = board.GetCell(col, currentRow);

            if (cell != null && cell.Unit == null) {
                moveCell = cell;
            }
            else {
                break;
            }
        }

        Debug.Log($"MoveVertical: {moveCell.position.col}, {moveCell.position.row}");
        ForceMove(target, moveCell);
    }

    private void MoveBackline(BaseUnit target) {
        Debug.Log("Move to backline");
        Cell currentCell = target.CurrentCell;
        int backLineCol = target.Owner == CharacterTypes.Player ? board.Column - 1 : 0;
        int backLineRow = currentCell.position.row;
        int currentCol = currentCell.position.col;
        Cell moveCell = currentCell;
        
        for (int col = backLineCol; col != currentCol; col += target.Owner == CharacterTypes.Player ? -1 : 1) {
            Cell cell = board.GetCell(col, backLineRow);

            if (cell != null && cell.Unit == null) {
                moveCell = cell;
                break;
            }
        }

        Debug.Log($"Move to backline: {moveCell.position.col}, {moveCell.position.row}");

        ForceMove(target, moveCell);
    }

    private void ForceMove(BaseUnit target, Cell moveCell, float overshoot = 0.5f) {
        if (moveCell != null && moveCell.Unit == null) {
            target.ForceMove(moveCell, 0.5f);
        }
    }
}

