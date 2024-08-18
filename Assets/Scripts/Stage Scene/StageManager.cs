using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager> {
    private List<EnemyData> _allEnemyData = new List<EnemyData>();
    [SerializeField] private List<StageData> _stages = new List<StageData>();
    private int _currentStageIndex;
    private int _stageAmount = 5;

    public StageData CurrentStage { get => _stages[_currentStageIndex]; }
    public List<StageData> Stages { get => _stages; }
    public int CurrentStageIndex {  get => _currentStageIndex; }

    protected override void Awake() {
        base.Awake();

        _allEnemyData = Resources.LoadAll<EnemyData>("Scriptable Objects/Enemy").ToList();

        InitStages();
    }

    private void InitStages() {
        _stages.Add(new StageData());
        _stages[0].stageIndex = 0;

        for (int i = 1; i <= _stageAmount; i++) {
            EnemyData enemyData = _allEnemyData[Random.Range(0, _allEnemyData.Count)];
            EnemyStageData enemyStage = new EnemyStageData(enemyData);
            enemyStage.stageIndex = i;

            _stages.Add(enemyStage);

            _currentStageIndex = 0;
        }
    }

    public void MoveForward() {
        _currentStageIndex++;
    }
}

public class StageData {
    public int stageIndex;
}

public class EnemyStageData : StageData {
    public EnemyData enemyData;

    public EnemyStageData(EnemyData enemyData) {
        this.enemyData = enemyData;
    }
}