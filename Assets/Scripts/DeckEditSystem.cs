using Assets.Scripts;
using Card;
using System.Collections.Generic;
using UnityEngine;

public class DeckEditSystem : MonoBehaviour
{
    private Player _player;
    [SerializeField] private CardSystem _cardSystem;
    private List<CardData> _deck;
    private List<CardData> _extraDeck;
    private Vector2 _cardSize;
    [SerializeField] private Transform extraCardDisplayParent;
    [SerializeField] private Transform deckDisplayParent;
    [SerializeField] private GameObject displayCardPrefab; // ī�� �������� �߰�

    // Start is called before the first frame update
    void Start()
    {
        _player = Player.Instance;
        _deck = _player.Deck;
        if (_deck == null)
        {
            Debug.LogError("Player's deck is null!");
            return;
        }
        _extraDeck = _player.ExtraDeck;
        _cardSize = displayCardPrefab.GetComponent<RectTransform>().rect.size; // RectTransform ���

        DisplayExtraCards();
        DisplayDeckCards();
    }

    private void DisplayExtraCards()
    {
        DisplayDeck(extraCardDisplayParent, _extraDeck, AddCardToDeck);
    }

    private void DisplayDeckCards()
    {
        DisplayDeck(deckDisplayParent, _deck, RemoveCardFromDeck);
    }

    private void DisplayDeck(Transform display, List<CardData> deck, UnityEngine.Events.UnityAction<CardData> clickAction)
    {
        foreach (Transform child in display)
        {
            if (child.CompareTag("CardPrefab"))
            {
                Destroy(child.gameObject);
            }
        }

        int rows = 3;
        int cols = 5;
        float spacingX = _cardSize.x * 0.7f; // ī�� ������ ������ ����
        float spacingY = _cardSize.y * 0.7f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int cardIndex = i * cols + j;
                if (cardIndex >= deck.Count) // ī�� ���� 15�� �̸��� ��� ����
                    return;

                CardData card = deck[cardIndex]; // ������ ī�� ��������

                // Instantiate UI card using prefab
                GameObject cardInstance = Instantiate(displayCardPrefab, display);
                RectTransform cardRectTransform = cardInstance.GetComponent<RectTransform>();

                // ī�� ������ ����
                DisplayCard displayCard = cardInstance.GetComponent<DisplayCard>();
                displayCard.Init(card);
                displayCard.SetOnClickListener(() => clickAction(card));

                // Set parent to deckDisplayParent and reset transform
                cardRectTransform.SetParent(display, false);
                cardRectTransform.localScale = new Vector3(0.6f, 0.6f, 1); // ������ ũ�� ����

                // Set position within the grid
                Vector2 position = new Vector2(j * spacingX, -i * spacingY);
                cardRectTransform.anchoredPosition = position;
            }
        }
    }

    private void AddCardToDeck(CardData card)
    {
        if (_deck.Count < 15) // ���� �ִ� 15����� �߰� ����
        {
            Debug.Log("Add card");
            _deck.Add(card);
            DisplayDeckCards();
        }
        else
        {
            Debug.Log("���� 15�� ���Ͽ��� ��");
        }

        _player.SetDeck(_deck);
    }

    private void RemoveCardFromDeck(CardData card)
    {
        if (_deck.Count > 5) // �ּ� 5�� �̻��̾�� ���� ����
        {
            Debug.Log("remove card");
            _deck.Remove(card);
            _extraDeck.Add(card); // ����Ʈ�� ���� ī�� �߰�
            DisplayDeckCards();
            DisplayExtraCards(); // ����Ʈ�� �� ����
        }
        else
        {
            Debug.Log("���� 5�� �̻� �̾�� ��");
        }

        _player.SetDeck(_deck);
        _player.SetExtraDeck(_extraDeck); // �÷��̾� ����Ʈ�� �� ������Ʈ
    }
}
