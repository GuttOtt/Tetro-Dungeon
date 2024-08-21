using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageNode : MonoBehaviour, IPointerClickHandler {
    private EnemyData _enemyData;

    public Action onPointerClick;

    public EnemyData EnemyData { get => _enemyData; }

    public void Init(StageData data) {
        if (data as EnemyStageData != null) {
            _enemyData = (data as EnemyStageData).enemyData;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        onPointerClick?.Invoke();
    }
}
