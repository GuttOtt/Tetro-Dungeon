using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager> {
    [SerializeField] private List<EnemyData> _allEnemyData = new List<EnemyData>();
    private List<BossData> _allBossData = new List<BossData>();
    [SerializeField] private List<StageEnum> _stageEnums = new List<StageEnum>();
    [SerializeField] public List<StageData> _stages = new List<StageData>();
    private int _currentStageIndex;
    private int _stageAmount = 5;

    public StageData CurrentStage { get => _stages[_currentStageIndex]; }
    public List<StageData> Stages { get => _stages; }
    public int CurrentStageIndex {  get => _currentStageIndex; }
    public EnemyData CurrentEnemyData {
        get {
            if (CurrentStage is not EnemyStageData) 
                return null;
            
            else 
                return (CurrentStage as EnemyStageData).enemyData;
        }
    }

    protected override void Awake() {
        base.Awake();

        _allEnemyData = Resources.LoadAll<EnemyData>("Scriptable Objects/Enemy/Normal").ToList();
        _allBossData = Resources.LoadAll<BossData>("Scriptable Objects/Enemy/Boss").ToList();

        InitStages();
    }

    private void InitStages() {
        for (int i = 0; i < _stageEnums.Count; i++) {
            switch (_stageEnums[i]) {
                case StageEnum.Starting:
                    AddStartingStage();
                    break;
                case StageEnum.Enemy:
                    AddEnemyStage();
                    break;

                case StageEnum.Boss:
                    AddBossStage();
                    break;
            }
        }
    }

    private void AddStartingStage() {
        if (_stages.Count > 0) {
            Debug.LogError("스타팅 스테이지는 항상 처음에 배치되어야 합니다.");
            return;
        }

        _stages.Add(new StartingStageData());
        _stages[0].stageIndex = 0;
    }
    
    private void AddEnemyStage() {
        EnemyData enemyData = _allEnemyData[UnityEngine.Random.Range(0, _allEnemyData.Count)];
        EnemyStageData enemyStage = new EnemyStageData(enemyData);
        enemyStage.stageIndex = _stages.Count;

        _stages.Add(enemyStage);
    }

    private void AddBossStage() {
        EnemyData enemyData = _allBossData[UnityEngine.Random.Range(0, _allBossData.Count)];
        EnemyStageData bossStage = new BossStageData(enemyData);
        bossStage.stageIndex = _stages.Count;

        _stages.Add(bossStage);
    }

    public void MoveForward() {
        _currentStageIndex++;
    }
}

[Serializable]
public class StageData {
    public int stageIndex;
}

[Serializable]
public class StartingStageData : StageData {

}

[Serializable]
public class EnemyStageData : StageData {
    public EnemyData enemyData;

    public EnemyStageData(EnemyData enemyData) {
        this.enemyData = enemyData;
    }
}

[Serializable]
public class BossStageData : EnemyStageData {
    public BossStageData(EnemyData enemyData) : base(enemyData) {
        this.enemyData = enemyData;
    }
}

public enum StageEnum
{
    Starting = 0, Enemy = 1, Boss = 2,
}