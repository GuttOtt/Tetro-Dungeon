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
    [SerializeField] float spacingX = 0.3f;  // X�� ������ ������ ������ ���� (��: 100 �ȼ�)
    [SerializeField] float spacingY = 0.25f;  // Y�� ������ ������ ������ ���� (��: 100 �ȼ�)

    private TroopCard selectedTroopCard;
    private UnitConfig selectedUnitConfig;
    private GameObject displayCardInstance;

    private Vector2 _blockSize;

    private UnitBlockDrawer _unitBlockMarker { get => cardSelector.UnitblockMarker; }

    private void Start()
    {
        // OK ��ư�� Back ��ư�� �����ʸ� �߰��մϴ�.
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
        int blockCount = Mathf.Min(rows * cols, blocks.Count);  // ��ġ�� ���� ���� ����
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

                block.transform.SetParent(troopCardPanel.transform, false);  // �θ� �����ϰ� ���� ������ ����
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                var cells = _unitBlockMarker.DrawBlock(blocks[index].Polyomino, troopCardPanel);

                foreach (var cell in cells)
                {
                    cell.transform.SetParent(block.transform, false);  // block�� �ڽ����� �����ϰ� ���� ������ ����
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
        int unitCount = Mathf.Min(rows * cols, Units.Count);  // ��ġ�� ���� ���� ����

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

                unit.transform.SetParent(unitConfigPanel.transform, false);  // �θ� �����ϰ� ���� ������ ����
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
            // ������ DisplayCard �ν��Ͻ��� �����մϴ�.
            if (displayCardInstance != null)
            {
                Destroy(displayCardInstance);
            }

            // ���� ���յ� DisplayCard�� �����մϴ�.
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

    private List<TroopCard> GetTroopCards()
    {
        return Player.Instance.TroopCards;
    }

    private List<UnitConfig> GetUnitConfigs()
    {
        return Player.Instance.Configs;
    }
}
