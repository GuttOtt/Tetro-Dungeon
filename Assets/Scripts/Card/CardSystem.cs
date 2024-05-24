using Card;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    [SerializeField] private UnitBlockDrawer _unitBlockMarker;
    private List<ICard> cards;
    private Deck deck;
    private ICard _selectedCard;

    [SerializeField]
    private List<UnitConfig> _unitPool = new List<UnitConfig>();
    [SerializeField]
    private BaseCard _cardPrefab;
    [SerializeField]
    private int deckNumber;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

    private void Update() {
        SelectCard();
        PlayCard();
        MoveUnitBlockMarker();
    }

    public BaseCard CreateCard(UnitConfig unitConfig, Polyomino polyomino) {
        BaseCard card = Instantiate(_cardPrefab);
        card.Init(unitConfig, polyomino);
        return card;
    }

    public void DrawCard() {

    }

    public void ShuffleDeck() {

    }

    public void SetDeck(int deckAmount) {
        for (int i = 0; i < deckAmount; i++) {
            UnitConfig config = _unitPool[Random.Range(0, _unitPool.Count)];
            Polyomino polyomino = Polyomino.GetRandomPolyomino();
            BaseCard card = CreateCard(config, polyomino);
            deck.AddCard(card);
        }
    }

    private void SelectCard() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Clicked");
            _selectedCard = Utils.Pick<BaseCard>();

            //카드를 선택했다면 카드에 맞는 유닛 블럭 마커를 생성
            if (_selectedCard != null) {
                Debug.Log("Selected");
                _unitBlockMarker.gameObject.SetActive(true);
                _unitBlockMarker.Draw(_selectedCard.Polyomino, _selectedCard.UnitConfig);
            }
        }
    }

    private void PlayCard() {
        if (_selectedCard != null && Input.GetMouseButtonUp(0)) {

            Vector3 topLeftPos = _unitBlockMarker.GetTopLeftPosition();
            Cell topLeftCell = Utils.Pick<Cell>(topLeftPos);
            Debug.Log("Cell: " + topLeftCell);

            if (topLeftCell != null) {
                Board board = _gameManager.GetSystem<Board>();
                board.Place(topLeftCell, _selectedCard.Polyomino, _selectedCard.UnitConfig);
            }

            //Unit Block Marker를 Clear하고 비활성화
            _unitBlockMarker.Clear();
            _unitBlockMarker.gameObject.SetActive(false);

            _selectedCard = null;
        }
    }

    private void MoveUnitBlockMarker() {
        //UnitBlockMarker가 활성화 되어 있다면, 마우스를 따라 움직이도록 함
        if (_unitBlockMarker.gameObject.activeSelf) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = _unitBlockMarker.transform.position.z;
            _unitBlockMarker.transform.position = mousePosition;
        }
    }
}
