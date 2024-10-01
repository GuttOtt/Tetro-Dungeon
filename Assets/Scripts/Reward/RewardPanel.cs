using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BlockCard BlockCard;
    public UnitConfig UnitConfig;
    public Action onClick;

    [SerializeField] private Image panelImage; // rewardPanel의 배경 이미지
    private Color originalColor; // 원래 색상 저장
    private GameObject selectedItem; // 선택된 아이템 (BlockCard나 UnitConfig)
    private bool clicked = false;

    private void Start()
    {
        // 패널의 배경 이미지를 가져오고, 원래 색상을 저장
        if (panelImage == null)
        {
            panelImage = GetComponent<Image>();
        }
        originalColor = panelImage.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();

       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 패널 위로 올라갔을 때 툴팁을 표시
        BlockCard blockCard = GetComponentInChildren<BlockCard>();
        UnitConfigUIDrawer unit = GetComponentInChildren<UnitConfigUIDrawer>();

        if (blockCard != null)
        {
            blockCard.TooltipPrefab.SetActive(true);
        }
        if (unit != null)
        {
            unit._tooltip.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 패널을 벗어났을 때 툴팁을 숨김
        BlockCard blockCard = GetComponentInChildren<BlockCard>();
        UnitConfigUIDrawer unit = GetComponentInChildren<UnitConfigUIDrawer>();

        if (blockCard != null)
        {
            blockCard.TooltipPrefab.SetActive(false);
        }
        if (unit != null)
        {
            unit._tooltip.SetActive(false);
        }
    }


    public void ChangeColor(Color color) {
        panelImage.color = color;
    }

    public void Reset()
    {
        ResetPanelColor();
        ResetClick();
        ResetSelectedItem();
    }

    public void ResetPanelColor() => panelImage.color = originalColor;

    public void ResetClick() => clicked = false;

    internal void ResetSelectedItem() => selectedItem = null;


}
