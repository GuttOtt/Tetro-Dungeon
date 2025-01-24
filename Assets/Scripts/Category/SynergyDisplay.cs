using TMPro;
using UnityEngine;
using EnumTypes;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SynergyDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _synergyText;
    [SerializeField] private Image _synergyImage;
    [SerializeField] private GameObject _descriptionPanel;
    [SerializeField] private TMP_Text _descriptionText;
    private SynergyTypes _synergyType;
    private BaseSynergy _synergyData;

    public void Init(SynergyTypes type, BaseSynergy data)
    {
        _synergyType = type;
        _synergyData = data;
        _synergyText.richText = true;
        
        // Description 패널 초기 설정
        _descriptionPanel.SetActive(false);
        _descriptionText.text = _synergyData.description;

        // 이벤트 트리거 설정
        EventTrigger trigger = _synergyImage.gameObject.AddComponent<EventTrigger>();
        
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { OnPointerEnter(); });
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnPointerExit(); });
        trigger.triggers.Add(exitEntry);
    }

    private void OnPointerEnter()
    {
        _descriptionPanel.SetActive(true);
    }

    private void OnPointerExit()
    {
        _descriptionPanel.SetActive(false);
    }

    public void UpdateDisplay(int synergyCount)
    {
        if (synergyCount <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        List<int> thresholds = new List<int>(_synergyData.synergyCountThresholds);
        thresholds.Sort();

        string thresholdsText = "";
        foreach (int threshold in thresholds)
        {
            string color = synergyCount >= threshold ? "#000000" : "#969696";
            thresholdsText += $"<color={color}>{threshold}</color> ";
        }

        _synergyText.text = $"{_synergyType}: {synergyCount}\n[{thresholdsText.TrimEnd()}]";
    }
}