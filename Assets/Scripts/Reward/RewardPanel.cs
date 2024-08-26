using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
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
        if (!clicked)
        {
            clicked = true;
            // 선택된 아이템을 패널에 포함된 BlockCard나 UnitConfig로 설정
            BlockCard blockCard = GetComponentInChildren<BlockCard>();
            UnitConfigUIDrawer unit = GetComponentInChildren<UnitConfigUIDrawer>();
            DeselectAllPanels();

            panelImage.color = new Color(0.5f, 1f, 0.5f, 0.5f); // 연한 녹색

            if (blockCard != null)
            {
                selectedItem = blockCard.gameObject;
            }
            else if (unit != null)
            {
                selectedItem = unit.gameObject;
            }

        }
        else
        {
            Reset();
        }
       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 패널 위로 올라갔을 때 툴팁을 표시
        BlockCard blockCard = GetComponentInChildren<BlockCard>();
        UnitConfigUIDrawer unit = GetComponentInChildren<UnitConfigUIDrawer>();

        //if (blockCard != null)
        //{
        //    ShowTooltip(blockCard.GetTooltipInfo());
        //}
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

        //if (blockCard != null)
        //{
        //    ShowTooltip(blockCard.GetTooltipInfo());
        //}
        if (unit != null)
        {
            unit._tooltip.SetActive(false);
        }
    }

    // 모든 패널들의 색상을 원래 색상으로 돌리는 함수
    private void DeselectAllPanels()
    {
        RewardPanel[] allPanels = FindObjectsOfType<RewardPanel>();
        foreach (RewardPanel panel in allPanels)
        {
            panel.Reset();
        }
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

    public GameObject GetSelectedItem() => selectedItem;


}
