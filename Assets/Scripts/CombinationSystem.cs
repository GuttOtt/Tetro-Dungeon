using Assets.Scripts;
using Card;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class CombinationSystem : MonoBehaviour
{
    [SerializeField] private Transform troopCardPanel;
    [SerializeField] private Transform unitConfigPanel;
    [SerializeField] private GameObject combinationPanel;

    [SerializeField] private GameObject troopCardPrefab;
    [SerializeField] private GameObject unitConfigPrefab;
    [SerializeField] private GameObject displayCardPrefab;

    [SerializeField] private Button okButton;
    [SerializeField] private Button backButton;
    [SerializeField] private CardSelector cardSelector;

    [SerializeField] int rows = 5;
    [SerializeField] int cols = 3;
    [SerializeField] float spacingX = 0.3f;  // X축 간격을 고정된 값으로 설정 (예: 100 픽셀)
    [SerializeField] float spacingY = 0.25f;  // Y축 간격을 고정된 값으로 설정 (예: 100 픽셀)

    private TroopCard selectedTroopCard;
    private UnitConfig selectedUnitConfig;
    private GameObject displayCardInstance;

    private Vector2 _blockSize;

    private UnitBlockDrawer _unitBlockMarker { get => cardSelector.UnitblockMarker; }

    private void Start()
    {
        // OK 버튼과 Back 버튼에 리스너를 추가합니다.
        okButton.onClick.AddListener(OnOkButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);

        _blockSize = troopCardPrefab.GetComponent<SpriteRenderer>().size;
        DisplayPanels();
    }

    private void DisplayPanels()
    {
        DisplayBlockCards(rows, cols, spacingX, spacingY);
        DisplayUnitCards(rows, cols, spacingX, spacingY);
    }

    private void DisplayBlockCards(int rows, int cols, float spacingX, float spacingY)
    {
        var blocks = GetTroopCards();
        int blockCount = Mathf.Min(rows * cols, blocks.Count);  // 배치할 블럭의 수를 제한
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;
                if (index >= blockCount)
                    break;

                GameObject block = new GameObject("Block");
                block.AddComponent<Draggable>();
                block.GetComponent<Draggable>().SetDestination(combinationPanel);
                block.AddComponent<BoxCollider>();

                block.transform.SetParent(troopCardPanel.transform, false);  // 부모를 설정하고 로컬 포지션 유지
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                var cells = _unitBlockMarker.DrawBlock(blocks[index].Polyomino, troopCardPanel);

                foreach (var cell in cells)
                {
                    cell.transform.SetParent(block.transform, false);  // block의 자식으로 설정하고 로컬 포지션 유지
                }

                float startX = -(cols - 1) * spacingX / 2;
                float startY = (rows - 1) * spacingY / 2;
                Vector2 localPosition = new Vector2(startX + j * spacingX, startY - i * spacingY);
                block.transform.localPosition = localPosition;
            }
        }
    }
    private void DisplayUnitCards(int rows, int cols, float spacingX, float spacingY)
    {
        var Units = GetUnitConfigs();
        int unitCount = Mathf.Min(rows * cols, Units.Count);  // 배치할 블럭의 수를 제한

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;
                if (index >= unitCount)
                    break;

                GameObject unit = Instantiate(unitConfigPrefab, unitConfigPanel);
                unit.AddComponent<UnitDrawer>();
                unit.GetComponent<UnitDrawer>().Draw(Units[index]);
                unit.AddComponent<Draggable>();
                unit.GetComponent<Draggable>().SetDestination(combinationPanel);

                unit.transform.SetParent(unitConfigPanel.transform, false);  // 부모를 설정하고 로컬 포지션 유지
                unit.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                float startX = -(cols - 1) * spacingX / 2;
                float startY = (rows - 1) * spacingY / 2;
                Vector2 localPosition = new Vector2(startX + j * spacingX, startY - i * spacingY);
                unit.transform.localPosition = localPosition;
            }
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
            displayCardInstance = Instantiate(displayCardPrefab, combinationPanel.transform);
            var cardData = new CardData(selectedUnitConfig, selectedTroopCard);
            displayCardInstance.GetComponent<DisplayCard>().Init(cardData);
        }
    }

    private void OnOkButtonClicked()
    {
        if (selectedTroopCard != null && selectedUnitConfig != null)
        {
            var cardData = new CardData(selectedUnitConfig, selectedTroopCard);
            Player.Instance.ExtraDeck.Add(cardData);
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

    private List<TroopCard> GetTroopCards()
    {
        return Player.Instance.TroopCards;
    }

    private List<UnitConfig> GetUnitConfigs()
    {
        return Player.Instance.Configs;
    }
}
