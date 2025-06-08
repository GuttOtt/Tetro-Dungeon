using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ShopCharacterSlot : MonoBehaviour
{
    private CharacterBlockConfig characterBlockConfig;
    [SerializeField] private SpriteRenderer illust;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private ShapeDrawer shapeDrawer;
    private Transform shapeParent;
    private int cost;

    public Action<ShopCharacterSlot> onClick;


    public int Cost => cost;
    public CharacterBlockConfig CharacterBlockConfig => characterBlockConfig;


    private void Update()
    {
        if (characterBlockConfig != null &&Input.GetMouseButtonDown(0) && Utils.Pick<ShopCharacterSlot>() == this)
        {
            onClick?.Invoke(this);
        }
    }

    public void SetCharacterBlock(CharacterBlockConfig config, int cost)
    {
        characterBlockConfig = config;
        illust.sprite = characterBlockConfig.Illust;
        this.cost = cost;
        costText.text = cost.ToString() + "G";

        // To Do: Shape Drawing
        if (shapeDrawer == null) return;
        bool[,] shape = characterBlockConfig.GetShape(1).GetCells();
        shapeDrawer.DrawShape(shape);
    }

    public void SetEmpty()
    {
        characterBlockConfig = null;
        illust.sprite = null;
        costText.text = "";
        shapeDrawer.Clear();
    }
}
