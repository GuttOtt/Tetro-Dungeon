using Card;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class Discards : MonoBehaviour, ICardPile {
    #region private members
    private List<ICard> _discards = new List<ICard>();
    [SerializeField] private Transform _discardsParent;
    #endregion

    public void AddCard(ICard card) {
        _discards.Add(card);
        BaseCard baseCard = card as BaseCard;
        baseCard.transform.parent = _discardsParent;
        baseCard.transform.localPosition = Vector3.zero;
    }

    public void RemoveCard(ICard card) {
        if (!_discards.Contains(card))
            return;
        _discards.Remove(card);
    }

    public List<ICard> GetAllCard() {
        return _discards.ToList();
    }

    public void Clear() {
        _discards.Clear();
    }
}
