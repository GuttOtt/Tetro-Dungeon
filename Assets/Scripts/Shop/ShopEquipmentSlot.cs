using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopEquipmentSlot : MonoBehaviour
{
    private Equipment equipment;
    private int cost;
    [SerializeField] private SpriteRenderer illust;
    [SerializeField] private Vector2 illustSize;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private ShapeDrawer shapeDrawer;

    public int Cost => cost;
    public Equipment Equipment => equipment;

    public event Action<ShopEquipmentSlot> onClick;

    private void Update()
    {
        if (equipment != null && Input.GetMouseButtonDown(0) && Utils.Pick<ShopEquipmentSlot>() == this)
        {
            onClick?.Invoke(this);
        }
    }

    public void SetEquipment(Equipment equipment, int cost)
    {
        this.equipment = equipment;
        equipment.transform.parent = transform;
        equipment.transform.localPosition = Vector3.zero;
        equipment.gameObject.SetActive(false);

        this.cost = cost;
        costText.text = cost.ToString() + "G";

        illust.sprite = equipment.Config.Sprite;
        Vector2 spriteSize = illust.sprite.bounds.size;
        float larger = Mathf.Max(spriteSize.x, spriteSize.y);
        float scale = illustSize.x / larger;
        illust.transform.localScale = new Vector3(scale, scale, 1);
        
        shapeDrawer.DrawShape(equipment.Config.Shape.GetCells());
    }

    public void SetEmpty()
    {
        equipment = null;
        costText.text = "";
        illust.sprite = null;
        shapeDrawer.Clear();
    }
}
