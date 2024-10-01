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
        onClick?.Invoke();

       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� �г� ���� �ö��� �� ������ ǥ��
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
        // ���콺�� �г��� ����� �� ������ ����
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
