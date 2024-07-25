using Assets.Scripts;
using Card;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deck : MonoBehaviour, ICardPile
{
    [SerializeField] private Transform _deckParent;
    [SerializeField] private Discards _discards;
    [SerializeField] private List<ICard> _decks;

    public int CardCount { get { return _decks.Count; } }
    public List<ICard> Decks { get {  return _decks; } }

    private void Awake()
    {
        NewDeck();
        // �̸����� DeckParent ���� ������Ʈ�� ã���ϴ�.
        GameObject deckParentObject = GameObject.Find("Deck Parent");
        if (deckParentObject != null)
        {
            _deckParent = deckParentObject.transform;
        }
        else
        {
            Debug.LogError("DeckParent game object not found!");
        }
    }

    public void AddCard(ICard card)
    {
        BaseCard baseCard = card as BaseCard;
        _decks.Add(baseCard);
        baseCard.transform.parent = _deckParent;
        baseCard.transform.localPosition = Vector3.zero;
    }

    public void RemoveCard(ICard card)
    {
        if (_decks.Contains(card))
        {
            _decks.Remove(card);
        }
    }

    public ICard Draw()
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

        if (_decks.Count == 0)
            throw new Exception("���� ī�� ����");

        ICard topCard = _decks[0];
        RemoveCard(topCard);

        return topCard;
    }

    // ���� ���� �ִ� ī�带 ��ȯ�ϴ� �Լ�
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

    public void NewDeck()
    {
        _decks = new List<ICard>();
    }

}
