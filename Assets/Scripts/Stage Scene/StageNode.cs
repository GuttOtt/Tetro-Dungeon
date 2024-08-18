using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private EnemyData _enemyData;

    public Action onPointerEnter, onPointerExit;

    public EnemyData EnemyData { get => _enemyData; }

    public void Init(StageData data) {
        if (data as EnemyStageData != null) {
            _enemyData = (data as EnemyStageData).enemyData;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        onPointerEnter.Invoke();
        Debug.Log("onHover");
    }

    public void OnPointerExit(PointerEventData eventData) {
        onPointerExit?.Invoke();
    }
}
