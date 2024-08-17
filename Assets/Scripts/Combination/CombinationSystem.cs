using Assets.Scripts;
using Card;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombinationSystem : MonoBehaviour
{
    [SerializeField] private Transform blockCardPanel;
    [SerializeField] private Transform unitConfigPanel;
    [SerializeField] private GameObject combinationPanel;

    [SerializeField] private GameObject blockCardPrefab;
    [SerializeField] private GameObject unitConfigPrefab;
    [SerializeField] private GameObject displayCardPrefab;

    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button backButton;
    [SerializeField] private CardSelector cardSelector;

    [SerializeField] int rows = 5;
    [SerializeField] int cols = 3;
    [SerializeField] float spacingX = 0.3f;  // X�� ������ ������ ������ ���� (��: 100 �ȼ�)
    [SerializeField] float spacingY = 0.25f;  // Y�� ������ ������ ������ ���� (��: 100 �ȼ�)

    private BlockCard selectedBlockCard;
    private UnitDrawer selectedUnitDrawer;
    private GameObject displayCardInstance;

    private List<Draggable> droppedCards = new List<Draggable>();

    private Vector2 _blockSize;

    private UnitBlockDrawer _unitBlockMarker { get => cardSelector.UnitblockMarker; }

    private void Start()
    {
        okButton.onClick.AddListener(OnOkButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);

        _blockSize = blockCardPrefab.GetComponent<SpriteRenderer>().size;
        DisplayPanels();
    }

    private void DisplayPanels()
    {
        DisplayBlockCards(rows, cols, spacingX, spacingY);
        DisplayUnitCards(rows, cols, spacingX, spacingY);
    }

    private void DisplayBlockCards(int rows, int cols, float spacingX, float spacingY)
    {
        var blocks = GetBlockCards();
        int blockCount = Mathf.Min(rows * cols, blocks.Count);  // ��ġ�� ���� ���� ����
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * cols + j;
                if (index >= blockCount)
                    break;

                GameObject block = new GameObject("Block");
                block.tag = "Draggable";

                block.AddComponent<BlockCard>();

                var blocksetting = block.GetComponent<BlockCard>();
                blocksetting.Init(blocks[index]);

                var draggble = block.AddComponent<Draggable>();
                draggble.SetDestination(combinationPanel);
                draggble.SetEndDragAction(DropOnCombinationPanel);

                block.AddComponent<BoxCollider>();

                block.transform.SetParent(blockCardPanel.transform, false);  // �θ� �����ϰ� ���� ������ ����
                block.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

                var cells = _unitBlockMarker.DrawBlock(blocks[index].Polyomino, blockCardPanel);

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
                unit.tag = "Draggable";

                unit.GetComponent<UnitDrawer>().Draw(Units[index]);

                var draggble = unit.AddComponent<Draggable>();

                draggble.SetDestination(combinationPanel);
                draggble.SetEndDragAction(DropOnCombinationPanel);

                unit.transform.SetParent(unitConfigPanel.transform, false);  // �θ� �����ϰ� ���� ������ ����
                unit.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

                float startX = -(cols - 1) * spacingX / 2;
                float startY = (rows - 1) * spacingY / 2;
                Vector2 localPosition = new Vector2(startX + j * spacingX, startY - i * spacingY);
                unit.transform.localPosition = localPosition;
            }
        }
    }

    private void TryCombine()
    {
        BlockCard blockCard;
        UnitDrawer unitDrawer = null;

        // ��ӵ� ī�� ����Ʈ���� BlockCard�� UnitConfig�� ã��
        foreach (var card in droppedCards)
        {
            try
            {
                blockCard = card.GetComponentInParent<BlockCard>();
            }
            catch
            {
                blockCard = null;
            }

            try
            {
                unitDrawer = card.GetComponentInParent<UnitDrawer>();
            }
            catch
            {
                unitDrawer = null;
            }

            if (blockCard != null)
            {
                Debug.Log("select blockcard");
                selectedBlockCard = blockCard;
            }
            else if (unitDrawer != null)
            {
                Debug.Log("select unitcard");
                selectedUnitDrawer = unitDrawer;
            }
        }

        if (selectedBlockCard != null && selectedUnitDrawer != null)
        {
            // ������ DisplayCard �ν��Ͻ��� �����մϴ�.
            if (displayCardInstance != null)
            {
                Destroy(displayCardInstance);
            }

            selectedBlockCard.gameObject.SetActive(false);
            selectedUnitDrawer.gameObject.SetActive(false);

            // ���� ���յ� DisplayCard�� �����մϴ�.
            displayCardInstance = Instantiate(displayCardPrefab, combinationPanel.transform);
            var cardData = new CardData(selectedUnitDrawer.UnitConfig, selectedBlockCard);
            displayCardInstance.GetComponent<DisplayCard>().Init(cardData);

            // ���÷��� ī���� ũ�⸦ �����մϴ�.
            displayCardInstance.transform.localScale = new Vector3(0.3f, 0.3f, 1f); // �ʿ��� ũ��� ����
        }
    }

    private void OnOkButtonClicked()
    {
        if (selectedBlockCard != null && selectedUnitDrawer != null)
        {
            var cardData = new CardData(selectedUnitDrawer.UnitConfig, selectedBlockCard);
            Player.Instance.ExtraDeck.Add(cardData);
            Debug.Log("Card added to ExtraDeck");
            // ���� �ʱ�ȭ
            selectedBlockCard = null;
            selectedUnitDrawer = null;
            droppedCards.Clear();

            if (displayCardInstance != null)
            {
                Destroy(displayCardInstance);
            }
        }
    }
    private void OnCancelButtonClicked()
    {
        if (displayCardInstance != null)
        {
            Destroy(displayCardInstance);
            displayCardInstance = null;
        }

        if (selectedBlockCard != null)
        {
            selectedBlockCard.gameObject.SetActive(true);
        }

        if (selectedUnitDrawer != null)
        {
            selectedUnitDrawer.gameObject.SetActive(true);
        }

        selectedBlockCard.GetComponent<Draggable>().ResetPosition();
        selectedUnitDrawer.GetComponent<Draggable>().ResetPosition();

        // ���� �ʱ�ȭ
        selectedBlockCard = null;
        selectedUnitDrawer = null;
        droppedCards.Clear();
    }

    private void DropOnCombinationPanel(Draggable draggable)
    {
        if (draggable.destination != null)
        {
            RectTransform panelRectTransform = draggable.destination.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform, Input.mousePosition, Camera.main))
            {
                Debug.Log("Dropped on Combination Panel");
                draggable.transform.SetParent(panelRectTransform, true);
                draggable.transform.localPosition = Vector3.zero;

                droppedCards.Add(draggable);
                TryCombine();
            }
            else
            {
                Debug.Log("Dropped outside Combination Panel");
                draggable.ResetPosition();
            }
        }
        else
        {
            draggable.ResetPosition();
            Debug.LogWarning("Destination panel is not set.");
        }
    }

    private List<BlockCard> GetBlockCards()
    {
        return Player.Instance.BlockCards;
    }

    private List<UnitConfig> GetUnitConfigs()
    {
        return Player.Instance.Configs;
    }
}
