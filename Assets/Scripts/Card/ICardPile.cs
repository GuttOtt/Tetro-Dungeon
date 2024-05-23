using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardPile {
    public bool AddCard(ICard card);
    public void RemoveCard(ICard card);
}
