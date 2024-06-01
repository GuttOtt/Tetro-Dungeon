using Card;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour, ICardPile
{
    [SerializeField] private Transform _deckParent;
    private List<ICard> _decks = new List<ICard>();

    public void AddCard(ICard card) {
        _decks.Add(card);
        BaseCard baseCard = card as BaseCard;
        baseCard.transform.parent = _deckParent;
        baseCard.transform.localPosition = Vector3.zero;
    }


    public void RemoveCard(ICard card) {
        _decks.Remove(card);
    }

    public ICard Draw() {
        if (_decks.Count == 0)
            return null;

        ICard topCard = _decks[0];
        RemoveCard(topCard);
        return topCard;
    }

    public void Shuffle() {
        _decks.Shuffle();
    }
}
