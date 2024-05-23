using Card;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    #region private members
    private IGameManager _gameManager;
    [SerializeField]
    private UnitBlockDrawer _unitBlockMarker;
    private List<ICard> cards;
    private Deck deck;
    private ICard _selectedCard;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

    private void Update() {
        SelectCard();
        PlayCard();
        MoveUnitBlockMarker();
    }

    void OnBeginBattle() {

    }

    public ICard CreateCard(UnitConfig unitConfig, Polyomino polyomino) {
        return null;
    }

    public void DrawCard() {

    }

    public void ShuffleDeck() {

    }

    private void SelectCard() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Clicked");
            _selectedCard = Utils.Pick<BaseCard>();

            //ī�带 �����ߴٸ� ī�忡 �´� ���� �� ��Ŀ�� ����
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

            //Unit Block Marker�� Clear�ϰ� ��Ȱ��ȭ
            _unitBlockMarker.Clear();
            _unitBlockMarker.gameObject.SetActive(false);

            _selectedCard = null;
        }
    }

    private void MoveUnitBlockMarker() {
        //UnitBlockMarker�� Ȱ��ȭ �Ǿ� �ִٸ�, ���콺�� ���� �����̵��� ��
        if (_unitBlockMarker.gameObject.activeSelf) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = _unitBlockMarker.transform.position.z;
            _unitBlockMarker.transform.position = mousePosition;
        }
    }
}
