using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffect : ScriptableObject {
    [SerializeField]
    private bool _isOnBattleBegin = false;

    [SerializeField]
    private bool _isCoolDownEffect = false;

    [SerializeField]
    private float _coolTime = 0f;

    private float _coolDownCount = 0f;

    public float CoolTime { get => _coolTime; }
    public float CoolDownCount { get => _coolDownCount; set => _coolDownCount = value; }

    public bool IsOnBattleBegin { get => _isOnBattleBegin; }
    public bool IsCoolDownEffect { get => _isCoolDownEffect; } 

    public virtual void OnBattleBeginEffect(TurnContext turnContext) {
        //자식 클래스에서 세부 구현
    }

    public virtual void CoolTimeEffect(TurnContext turnContext) {
        //자식 클래스에서 세부 구현
    }
}
