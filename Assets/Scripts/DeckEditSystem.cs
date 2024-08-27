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
    [SerializeField] private GameObject displayCardPrefab; // 카드 프리팹을 추가

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
        _cardSize = displayCardPrefab.GetComponent<RectTransform>().rect.size; // RectTransform 사용

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
        float spacingX = _cardSize.x * 0.7f; // 카드 사이의 간격을 줄임
        float spacingY = _cardSize.y * 0.7f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int cardIndex = i * cols + j;
                if (cardIndex >= deck.Count) // 카드 수가 15개 미만일 경우 방지
                    return;

                CardData card = deck[cardIndex]; // 덱에서 카드 가져오기

                // Instantiate UI card using prefab
                GameObject cardInstance = Instantiate(displayCardPrefab, display);
                RectTransform cardRectTransform = cardInstance.GetComponent<RectTransform>();

                // 카드 데이터 설정
                DisplayCard displayCard = cardInstance.GetComponent<DisplayCard>();
                displayCard.Init(card);
                displayCard.SetOnClickListener(() => clickAction(card));

                // Set parent to deckDisplayParent and reset transform
                cardRectTransform.SetParent(display, false);
                cardRectTransform.localScale = new Vector3(0.6f, 0.6f, 1); // 프리팹 크기 조정

                // Set position within the grid
                Vector2 position = new Vector2(j * spacingX, -i * spacingY);
                cardRectTransform.anchoredPosition = position;
            }
        }
    }

    private void AddCardToDeck(CardData card)
    {
        if (_deck.Count < 15) // 덱에 최대 15장까지 추가 가능
        {
            Debug.Log("Add card");
            _deck.Add(card);
            DisplayDeckCards();
        }
        else
        {
            Debug.Log("덱은 15장 이하여야 함");
        }

        _player.SetDeck(_deck);
    }

    private void RemoveCardFromDeck(CardData card)
    {
        if (_deck.Count > 5) // 최소 5장 이상이어야 제거 가능
        {
            Debug.Log("remove card");
            _deck.Remove(card);
            _extraDeck.Add(card); // 엑스트라 덱에 카드 추가
            DisplayDeckCards();
            DisplayExtraCards(); // 엑스트라 덱 갱신
        }
        else
        {
            Debug.Log("덱은 5장 이상 이어야 함");
        }

        _player.SetDeck(_deck);
        _player.SetExtraDeck(_extraDeck); // 플레이어 엑스트라 덱 업데이트
    }
}
