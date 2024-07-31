using Assets.Scripts;
using Card;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombinationSystem : MonoBehaviour
{
    [SerializeField] private Transform troopCardPanel;
    [SerializeField] private Transform unitConfigPanel;
    [SerializeField] private Transform combinationPanel;

    [SerializeField] private GameObject troopCardPrefab;
    [SerializeField] private GameObject unitConfigPrefab;
    [SerializeField] private GameObject displayCardPrefab;

    [SerializeField] private Button okButton;
    [SerializeField] private Button backButton;

    private TroopCard selectedTroopCard;
    private UnitConfig selectedUnitConfig;
    private GameObject displayCardInstance;

    private void Start()
    {
        // OK ��ư�� Back ��ư�� �����ʸ� �߰��մϴ�.
        okButton.onClick.AddListener(OnOkButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);

        DisplayPanels();
    }

    private void DisplayPanels()
    {
        // TroopCard�� UnitConfig�� �гο� �߰��ϴ� �ڵ带 �ۼ��մϴ�.
        // ����:
        foreach (var troop in GetTroopCards())
        {
            var item = Instantiate(troopCardPrefab, troopCardPanel);
            item.GetComponent<TroopCard>().Init(troop);
            //item.GetComponent<Draggable>().OnDraggedToCombinationPanel += OnPolyominoDragged;
        }

        foreach (var unitConfig in GetUnitConfigs())
        {
            var item = Instantiate(unitConfigPrefab, unitConfigPanel);
            item.GetComponent<UnitConfig>().Init(unitConfig);
            //item.GetComponent<Draggable>().OnDraggedToCombinationPanel += OnUnitConfigDragged;
        }
    }

    private void OnPolyominoDragged(TroopCard troop)
    {
        selectedTroopCard = troop;
        TryCombine();
    }

    private void OnUnitConfigDragged(UnitConfig unitConfig)
    {
        selectedUnitConfig = unitConfig;
        TryCombine();
    }

    private void TryCombine()
    {
        if (selectedTroopCard != null && selectedUnitConfig != null)
        {
            // ������ DisplayCard �ν��Ͻ��� �����մϴ�.
            if (displayCardInstance != null)
            {
                Destroy(displayCardInstance);
            }

            // ���� ���յ� DisplayCard�� �����մϴ�.
            displayCardInstance = Instantiate(displayCardPrefab, combinationPanel);
            var cardData = new CardData(selectedUnitConfig, selectedTroopCard);
            displayCardInstance.GetComponent<DisplayCard>().Init(cardData);
        }
    }

    private void OnOkButtonClicked()
    {
        if (selectedTroopCard != null && selectedUnitConfig != null)
        {
            var cardData = new CardData(selectedUnitConfig, selectedTroopCard);
            Player.instance.ExtraDeck.Add(cardData);
            Debug.Log("Card added to ExtraDeck");
            // ���� �ʱ�ȭ
            selectedTroopCard = null;
            selectedUnitConfig = null;
            if (displayCardInstance != null)
            {
                Destroy(displayCardInstance);
            }
        }
    }

    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("DeckEditScene");
    }

    // ���÷� ����� ���� ������ ������
    private List<TroopCard> GetTroopCards()
    {
        return Player.instance.TroopCards;
    }

    private List<UnitConfig> GetUnitConfigs()
    {
        return Player.instance.Configs;
    }
}
