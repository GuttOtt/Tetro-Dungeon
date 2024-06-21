using Card;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    private TroopCardSystem _troopCardSystem;

    [SerializeField]
    private UnitBlockDrawer _unitBlockMarker;

    private List<ICard> _cards;

    private Deck _deck;
    private Hand _hand;
    private Discards _discards;
    
    private ICard _selectedCard;


    [SerializeField]
    private List<UnitConfig> _unitPool = new List<UnitConfig>();
    [SerializeField]
    private BaseCard _cardPrefab;
    [SerializeField]
    private int deckNumber;

    //ī�带 ���콺�� ������ �� �ִ� ��������
    private bool _isInputOn = false;

    //���� ��ġ���� ���� polyomino
    private Polyomino _selectedPolyomino;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
        _troopCardSystem = _gameManager.GetSystem<TroopCardSystem>();

        _deck = GetComponent<Deck>();
        _hand = GetComponent<Hand>();
        _discards = GetComponent<Discards>();
    }

    private void Update() {
        SelectCard();
        PlayCard();
        MoveUnitBlockMarker();

        if (Input.GetKeyDown(KeyCode.Q)) {
            SpinUnitBlockMarker(false);
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            SpinUnitBlockMarker(true);
        }
    }

    public UnitConfig GetRandomUnitConfig() {
        return _unitPool[Random.Range(0, _unitPool.Count)];
    }

    public BaseCard CreateCard(UnitConfig unitConfig, TroopCard troopCard) {
        BaseCard card = Instantiate(_cardPrefab);
        card.Init(unitConfig, troopCard);
        return card;
    }

    #region Deck And Hand Methods
    public void SetDeck(int deckAmount) {
        for (int i = 0; i < deckAmount; i++) {
            UnitConfig config = _unitPool[Random.Range(0, _unitPool.Count)];
            TroopCard troopCard = _troopCardSystem.CreateRandomTroopCard();
            BaseCard card = CreateCard(config, troopCard);
            _deck.AddCard(card);
        }

        _deck.Shuffle();
    }
    public void DrawCard(int amount) {
        for (int i = 0; i < amount; i++) {
            DrawCard();
        }
    }

    public void DrawCard() {
        ICard card = _deck.Draw();
        _hand.AddCard(card);
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

        //ī�� ȣ�� ���� �ִϸ��̼� � ���⼭ �� �� �ֵ��� �Ѵ�
    }

    public void SetInputOff() {
        _isInputOn= false;

        //ī�� ȣ�� ���� �ִϸ��̼� � ���⼭ �� �� �ֵ��� �Ѵ�
    }

    private void SelectCard() {
        if (!_isInputOn) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            _selectedCard = Utils.Pick<BaseCard>();

            if (!_hand.IsContain(_selectedCard))
                return;

            //ī�带 �����ߴٸ� ī�忡 �´� ���� �� ��Ŀ�� ����
            if (_selectedCard != null) {
                Debug.Log("Selected");
                _unitBlockMarker.gameObject.SetActive(true);
                _unitBlockMarker.Draw(_selectedCard.Polyomino, _selectedCard.UnitConfig);
                _selectedPolyomino = _selectedCard.Polyomino;
            }
        }
    }

    private void PlayCard() {
        if (_selectedCard != null && Input.GetMouseButtonUp(0)) {

            Vector3 topLeftPos = _unitBlockMarker.GetTopLeftPosition();
            Cell topLeftCell = Utils.Pick<Cell>(topLeftPos);
            bool isPlaced = false;

            if (topLeftCell != null) {
                Board board = _gameManager.GetSystem<Board>();
                isPlaced = board.Place(topLeftCell, _selectedPolyomino, _selectedCard.UnitConfig);
            }

            //Unit Block Marker�� Clear�ϰ� ��Ȱ��ȭ
            _unitBlockMarker.Clear();
            _unitBlockMarker.gameObject.SetActive(false);

            //���� ��ġ�� ������ ��� ī�带 Discard�� ����
            if (isPlaced) {
                _hand.RemoveCard(_selectedCard);
                _discards.AddCard(_selectedCard);
            }

            _selectedCard = null;
        }
    }

    private void SpinUnitBlockMarker(bool clockwise) {
        //���� �� ��Ŀ�� ���ٸ� ����
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

    private void MoveUnitBlockMarker() {
        //UnitBlockMarker�� Ȱ��ȭ �Ǿ� �ִٸ�, ���콺�� ���� �����̵��� ��
        if (_unitBlockMarker.gameObject.activeSelf) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = _unitBlockMarker.transform.position.z;
            _unitBlockMarker.transform.position = mousePosition;
        }
    }
    #endregion
}
