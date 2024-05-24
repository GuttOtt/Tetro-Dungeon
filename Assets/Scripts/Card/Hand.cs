using Card;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour, ICardPile
{
    [SerializeField] private Transform handParent;
    [SerializeField] private List<ICard> hands;

    public void AddCard(ICard card) {
        hands.Add(card);
        BaseCard baseCard = card as BaseCard;
        baseCard.transform.parent = handParent;

        Arrange();
    }

    public void RemoveCard(ICard card) {

    }

    private void Arrange() {
        if (hands.Count == 0) {
            return;
        }

        Vector2 cardSize = (hands[0] as BaseCard).GetComponent<SpriteRenderer>().bounds.size;

        for (int i = 0; i < hands.Count; i++) {
            BaseCard card = hands[0] as BaseCard;
            card.transform.localPosition = new Vector2(i * cardSize.x, cardSize.y);
        }
    }
}
