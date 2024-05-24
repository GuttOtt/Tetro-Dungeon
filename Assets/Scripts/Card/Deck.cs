using Card;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour, ICardPile
{
    [SerializeField] private Transform deckParent;
    private List<ICard> decks;

    public void AddCard(ICard card) {
        decks.Add(card);
        BaseCard baseCard = card as BaseCard;
        baseCard.transform.parent = deckParent;
        baseCard.transform.localPosition = Vector3.zero;
    }

    public void RemoveCard(ICard card) {
        decks.Remove(card);
    }
}
