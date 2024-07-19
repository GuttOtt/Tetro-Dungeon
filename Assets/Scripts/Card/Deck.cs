using Card;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour, ICardPile
{
    [SerializeField] private Transform _deckParent;
    [SerializeField] private Discards _discards;
    private List<ICard> _decks = new List<ICard>();

    public int CardCount { get { return _decks.Count; } }
    public List<ICard> Decks { get {  return _decks; } }
    public void AddCard(ICard card) {
        _decks.Add(card);
        BaseCard baseCard = card as BaseCard;
        baseCard.transform.SetParent(_deckParent);
        baseCard.transform.localPosition = Vector3.zero;
    }

    public void RemoveCard(ICard card) {
        _decks.Remove(card);
    }

    public ICard Draw() {
        if (_decks.Count == 0) {
            foreach (ICard card in _discards.GetAllCard()) {
                AddCard(card);
            }
            _discards.Clear();
            Shuffle();
        }

        ICard topCard = _decks[0];
        RemoveCard(topCard);
        return topCard;
    }

    // 가장 위에 있는 카드를 반환하는 함수
    public ICard Peek()
    {
        if (_decks.Count == 0)
        {
            foreach (ICard card in _discards.GetAllCard())
            {
                AddCard(card);
            }
            _discards.Clear();
            Shuffle();
        }

        return _decks.Count > 0 ? _decks[0] : null;
    }

    public void Shuffle() {
        _decks.Shuffle();
    }

    public void Empty()
    {
        _decks.Clear();
    }
}
