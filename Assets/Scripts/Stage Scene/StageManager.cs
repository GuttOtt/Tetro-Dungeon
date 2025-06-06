using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager> {
    [SerializeField] private List<EnemyData> _allEnemyData = new List<EnemyData>();
    private List<EnemyData> _allBossData = new List<EnemyData>();
    [SerializeField] private List<StageEnum> _stageEnums = new List<StageEnum>();
    [SerializeField] public List<StageData> _stages = new List<StageData>();
    [SerializeField] private int _currentStageIndex = 0;
    //private int _stageAmount = 5;

    public StageData CurrentStage { 
        get {
            if (_stages.Count == 0) {
                return null;
            }
            return _stages[_currentStageIndex];
        }
    }
    public List<StageData> Stages { get => _stages; }
    public int CurrentStageIndex {  get => _currentStageIndex; }
    public bool IsLastStage { get => _currentStageIndex == _stages.Count - 1; }
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
        _allBossData = Resources.LoadAll<EnemyData>("Scriptable Objects/Enemy/Boss").ToList();

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
    public EnemyData enemyData;
}

[Serializable]
public class StartingStageData : StageData {

}

[Serializable]
public class EnemyStageData : StageData {

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