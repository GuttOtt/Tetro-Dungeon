using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Summon Skill Config", menuName = "ScriptableObjects/Skill/SummonSkillConfig")]
public class SummonSkillConfig : SkillConfig
{
    [SerializeField] private int _summonAmount;
    [SerializeField] private CharacterBlockConfig _configToSummon;

    public int SummonAmount { get => _summonAmount; }
    public CharacterBlockConfig ConfigToSummon { get => _configToSummon; }
}

public class SummonSkill: UnitSkill {
    private int _summonAmount;
    private CharacterBlockConfig _configToSummon;
    private CharacterBlockConfig _originalConfigToSummon;


    public int SummonAmount { get => _summonAmount; }
    public CharacterBlockConfig ConfigToSummon { get => _configToSummon; }
    
    public SummonSkill(SummonSkillConfig config) : base(config) {
        _summonAmount = config.SummonAmount;
        _configToSummon = config.ConfigToSummon;
        _originalConfigToSummon = config.ConfigToSummon;
    }

    public override void Activate(TurnContext turnContext, BaseUnit activator, BaseUnit target = null) {
        Summon(activator, turnContext.Board);
    }

    public override void Decorate(SkillConfig config)
    {
        if (config is SummonSkillConfig summonSkillConfig){
            _summonAmount += summonSkillConfig.SummonAmount;
            _configToSummon = summonSkillConfig.ConfigToSummon;
        }
        else {
            Debug.LogWarning("Invalid config type for SummonSkill.");
        }
    }

    public override void Undecorate(SkillConfig config) {
        if (config is SummonSkillConfig summonSkillConfig){
            _summonAmount -= summonSkillConfig.SummonAmount;
            _configToSummon = _originalConfigToSummon;
        }
        else {
            Debug.LogWarning("Invalid config type for SummonSkill.");
        }
    }

    private void Summon(BaseUnit activator, Board board) {
        CharacterTypes owner = activator.Owner;
        Cell[,] cells = owner == CharacterTypes.Player ? board.GetPlayerCells() : board.GetEnemyCells();

    // 1. Flatten the 2D array and filter out occupied cells
    List<Cell> availableCells = new List<Cell>();
    foreach (Cell cell in cells) {
        if (cell.Unit == null) {
            availableCells.Add(cell);
        }
    }

    // 2. Handle edge case where there are not enough available cells.
    int summonCount = Mathf.Min(_summonAmount, availableCells.Count);
    if (summonCount == 0) {
        Debug.LogWarning("No available cells to summon units.");
        return;
    }

    // 3. Shuffle the list of available cells using Fisher-Yates shuffle algorithm.
    for (int i = availableCells.Count - 1; i > 0; i--) {
        int j = UnityEngine.Random.Range(0, i + 1);
        Cell temp = availableCells[i];
        availableCells[i] = availableCells[j];
        availableCells[j] = temp;
    }

    // 4. Summon units on the first `summonCount` shuffled cells
    for (int i = 0; i < summonCount; i++) {
        Cell cell = availableCells[i];

        // Assuming UnitBlockSystem is accessible and handles instantiation:
        BaseUnit summonedUnit = board.SummonUnit(cell, _configToSummon, owner); 

        if (summonedUnit != null) {
           // You might want to add the summoned unit to your unitDic here if necessary.
           //  board.unitDic[owner].Add(summonedUnit);
            Debug.Log($"Summoned unit at: ({cell.position.col}, {cell.position.row})");
        } else {
            Debug.LogError($"Failed to summon unit at: ({cell.position.col}, {cell.position.row})");
        }
    }
    }
}