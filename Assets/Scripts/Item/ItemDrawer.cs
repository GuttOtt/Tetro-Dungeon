using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrawer : MonoBehaviour, IPointerClickHandler {
    [SerializeField]
    private TMP_Text _nameText, _descriptionText;

    [SerializeField]
    private PolyominoDrawer _polyominoDrawer;

    [SerializeField]
    private Item ForTest;

    public Action OnClick;

    private void Start () {
    }

    public void Draw(Item item) {
        _nameText?.SetText(item.Name);
        _descriptionText.SetText(item.Description);

        _polyominoDrawer.Draw(item.ShapeInt);
    }

    public void SetColor(Color color) {
        GetComponent<Image>().color = color;
    }



    public void OnPointerClick(PointerEventData eventData) {
        OnClick.Invoke();
    }
}
