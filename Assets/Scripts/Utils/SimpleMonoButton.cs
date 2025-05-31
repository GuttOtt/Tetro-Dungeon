using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleMonoButton : MonoBehaviour, IPointerClickHandler {
    public Action onClick;

    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke();
    }
}
