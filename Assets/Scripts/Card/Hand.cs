using Card;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour, ICardPile {
    #region private members
    [SerializeField] private Transform _handParent;
    [SerializeField] private List<ICard> _hands = new List<ICard>();
    private Vector2 _cardSize;
    #endregion

    public List<ICard> Hands { get => _hands; }

    public void AddCard(ICard card) {
        _hands.Add(card);
        BaseCard baseCard = card as BaseCard;
        baseCard.transform.SetParent(_handParent);

        Arrange();
    }

    public void RemoveCard(ICard card) {
        if (!_hands.Contains(card))
            return;

        _hands.Remove(card);
        Arrange();
    }

    public bool IsContain(ICard card) {
        return _hands.Contains(card);
    }

    private void Arrange() {
        if (_hands.Count == 0) {
            return;
        }

        if (_cardSize.x == 0) {
            _cardSize = (_hands[0] as BaseCard).GetComponent<SpriteRenderer>().bounds.size;
        }

        //Get origin
        float xOrigin = _hands.Count % 2 == 0 ? (_hands.Count / 2 - 0.5f) * -_cardSize.x
            : _hands.Count / 2 * -_cardSize.x;

        for (int i = 0; i < _hands.Count; i++) {
            BaseCard card = _hands[i] as BaseCard;
            card.transform.localPosition = new Vector2(xOrigin + i * _cardSize.x, 0);
        }
    }

}
