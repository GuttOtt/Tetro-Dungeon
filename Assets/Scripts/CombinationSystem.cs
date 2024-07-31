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
        // OK 버튼과 Back 버튼에 리스너를 추가합니다.
        okButton.onClick.AddListener(OnOkButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);

        DisplayPanels();
    }

    private void DisplayPanels()
    {
        // TroopCard와 UnitConfig를 패널에 추가하는 코드를 작성합니다.
        // 예시:
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
            // 기존의 DisplayCard 인스턴스를 제거합니다.
            if (displayCardInstance != null)
            {
                Destroy(displayCardInstance);
            }

            // 새로 조합된 DisplayCard를 생성합니다.
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
            // 선택 초기화
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

    // 예시로 사용할 더미 데이터 생성기
    private List<TroopCard> GetTroopCards()
    {
        return Player.instance.TroopCards;
    }

    private List<UnitConfig> GetUnitConfigs()
    {
        return Player.instance.Configs;
    }
}
