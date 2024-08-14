using Card;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CardSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    private TroopCardSystem _troopCardSystem;

    private UnitBlockDrawer _unitBlockMarker { get => _cardSelector.UnitblockMarker; }

    private Deck _deck;
    private Hand _hand;
    private Discards _discards;

    private ICard _selectedCard { get => _cardSelector.SelectedCard; }


    [SerializeField]
    private List<UnitConfig> _unitPool = new List<UnitConfig>();
    [SerializeField]
    private BaseCard _cardPrefab;
    [SerializeField]
    private int deckNumber;

    //카드를 마우스로 선택할 수 있는 상태인지
    private bool _isInputOn = false;

    //현재 배치중인 블럭의 polyomino
    private Polyomino _selectedPolyomino;

    //카드 셀렉터
    [SerializeField]
    private CardSelector _cardSelector;
    #endregion

    public Deck Deck { get { return _deck; } }


    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _troopCardSystem = _gameManager.GetSystem<TroopCardSystem>();

        _unitPool = Resources.LoadAll<UnitConfig>("Scriptable Objects/Unit").ToList();

        _deck = GetComponent<Deck>();
        _hand = GetComponent<Hand>();
        _discards = GetComponent<Discards>();
    }

    private void Update() {
        SelectCard();
        PlayCard();

        if (Input.GetKeyDown(KeyCode.Q)) {
            SpinUnitBlockMarker(false);
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            SpinUnitBlockMarker(true);
        }
    }

    public BaseCard CreateCard(UnitConfig unitConfig, BlockCard troopCard) {
        BaseCard card = Instantiate(_cardPrefab);
        card.Init(unitConfig, troopCard);
        return card;
    }
    public BaseCard CreateCard(CardData _card) {
        BaseCard card = Instantiate(_cardPrefab);
        card.Init(_card);
        return card;
    }

    #region Deck And Hand Methods
    public void SetDeck(int deckAmount) {
        for (int i = 0; i < deckAmount; i++) {
            UnitConfig config = _unitPool[Random.Range(0, _unitPool.Count)];
            BlockCard troopCard = _troopCardSystem.CreateRandomTroopCard();
            BaseCard card = CreateCard(config, troopCard);
            _deck.AddCard(card);
        }

        _deck.Shuffle();
    }

    public void SetDeck(List<CardData> deck) {
        foreach (CardData cardData in deck)
        {
            BaseCard card = CreateCard(cardData);
            _deck.AddCard(card);
        }
        _deck.Shuffle();
    }

    public Deck GetRandomDeck(int deckAmount)
    {
        var ret = new Deck();
        for (int i = 0; i < deckAmount; i++)
        {
            UnitConfig config = _unitPool[Random.Range(0, _unitPool.Count)];
            BlockCard troopCard = _troopCardSystem.CreateRandomTroopCard();
            BaseCard card = CreateCard(config, troopCard);
            ret.AddCard(card);
        }

        ret.Shuffle();
        return ret;
    }

    public void ClearDeck()
    {
        _deck.Empty();
    }
    public void NewDeck()
    {
        _deck.NewDeck();
    }

    public List<BlockCard> GetRandomTroopCard(int n)
    {
        var ret = new List<BlockCard>();
        for (int i = 0; i<n; i++)
        {
            ret.Add(_troopCardSystem.CreateRandomTroopCard());
        }
        return ret;
    }

    public void DrawCard(int amount) {
        for (int i = 0; i < amount; i++) {
            DrawCard();
        }
    }

    public void DrawCard() {
        try
        {
            ICard card = _deck.Draw();
            _hand.AddCard(card);
        }
        catch(System.Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    public void ShuffleDeck() {
        _deck.Shuffle();
    }

    public void DiscardAllHand() {
        List<ICard> temp = _hand.Hands.ToList();

        foreach (var card in temp) {
            _hand.RemoveCard(card);
            _discards.AddCard(card);
        }
    }
    #endregion


    #region Selecting and Playing Cards
    public void SetInputOn() {
        _isInputOn = true;

        //카드 호버 시의 애니메이션 등도 여기서 켤 수 있도록 한다
    }

    public void SetInputOff() {
        _isInputOn= false;

        //카드 호버 시의 애니메이션 등도 여기서 끌 수 있도록 한다
    }

    private void SelectCard() {
        if (!_isInputOn) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            // Check if the mouse was clicked over a UI element
            if (Utils.IsPointerOverUIObject())
            {
                Debug.Log("Clicked on the UI");
            }
            else
            {
                Debug.Log("noclick");

            }
            _cardSelector.Select();
          
            if (_selectedCard == null || !_hand.IsContain(_selectedCard))
                return;

            _selectedPolyomino = _selectedCard.Polyomino;
        }
    }

    private void PlayCard() {
        if (_selectedCard != null && Input.GetMouseButtonUp(0)) {

            Vector3 topLeftPos = _unitBlockMarker.GetTopLeftPosition();
            Cell topLeftCell = Utils.Pick<Cell>(topLeftPos);

            Board board = _gameManager.GetSystem<Board>();

            if (topLeftCell != null && board.IsPlacable(_selectedPolyomino, topLeftCell)) {
                UnitBlock unitBlock = board.Place(topLeftCell, _selectedPolyomino, _selectedCard.UnitConfig);

                //Troop의 OnPlace 효과 발동
                TurnContext turnContext = _gameManager.CreateTurnContext();
                _selectedCard.BlockCard.TroopEffect.OnPlace(turnContext, unitBlock);

                //카드를 Discard로 보냄
                _hand.RemoveCard(_selectedCard);
                _discards.AddCard(_selectedCard);
            }

            _cardSelector.Unselect();
        }
    }

    private void SpinUnitBlockMarker(bool clockwise) {
        //유닛 블럭 마커가 없다면 리턴
        if (!_unitBlockMarker.gameObject.activeSelf) {
            return;
        }

        Polyomino originPolyomino = _selectedPolyomino;
        Polyomino rotatedPolyomino;

        if (clockwise) {
            rotatedPolyomino = originPolyomino.ClockwiseSpin();
        }
        else {
            rotatedPolyomino = originPolyomino.AnticlockwiseSpin();
        }

        _unitBlockMarker.Draw(rotatedPolyomino, _selectedCard.UnitConfig);
        _selectedPolyomino = rotatedPolyomino;
    }

    #endregion
}
