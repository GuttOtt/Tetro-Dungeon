using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour, ICardPile
{
    private List<ICard> decks;

    public bool AddCard(ICard card) {
        decks.Add(card);
        return true;
    }

    public void RemoveCard(ICard card) {
        decks.Remove(card);
    }
}
