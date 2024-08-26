using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image panelImage; // rewardPanel�� ��� �̹���
    private Color originalColor; // ���� ���� ����
    private GameObject selectedItem; // ���õ� ������ (BlockCard�� UnitConfig)
    private bool clicked = false;

    private void Start()
    {
        // �г��� ��� �̹����� ��������, ���� ������ ����
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
            // ���õ� �������� �гο� ���Ե� BlockCard�� UnitConfig�� ����
            BlockCard blockCard = GetComponentInChildren<BlockCard>();
            UnitConfigUIDrawer unit = GetComponentInChildren<UnitConfigUIDrawer>();
            DeselectAllPanels();

            panelImage.color = new Color(0.5f, 1f, 0.5f, 0.5f); // ���� ���

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
        // ���콺�� �г� ���� �ö��� �� ������ ǥ��
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
        // ���콺�� �г��� ����� �� ������ ����
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

    // ��� �гε��� ������ ���� �������� ������ �Լ�
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
