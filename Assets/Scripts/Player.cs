using Card;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets.Scripts
{
    public class Player : Singleton<Player>
    {
        private List<UnitConfig> _configs = new List<UnitConfig>();
        private List<TroopCard> _troopCards = new List<TroopCard>();

        private List<CardData> _deck;
        private List<CardData> _extraDeck;

        [SerializeField]
        private List<UnitConfig> _unitPool = new List<UnitConfig>();
        [SerializeField]
        private BaseCard _cardPrefab;
        [SerializeField]
        private int deckNumber;
        [SerializeField]
        private List<TroopEffect> _allTroopEffect;
        private List<StatDecorator> _allStatDecorator;

        [SerializeField]
        private List<Item> _itemInUse = new List<Item>();
        private List<Item> _itemInInv = new List<Item>();

        public List<CardData> Deck { get { return _deck; } }
        public List<CardData> ExtraDeck { get { return _extraDeck; } }

        public List<UnitConfig> Configs {  get { return _configs; } }

        public List<Item> ItemInUse { get => _itemInUse; }
        public List<Item> ItemInInv { get => _itemInInv; }

        public List<TroopCard> TroopCards {  get { return _troopCards; } }

        protected override void Awake()
        {
            base.Awake();

            _configs = new List<UnitConfig>();
            _deck = new List<CardData>();
            _extraDeck = new List<CardData>();

            _unitPool = Resources.LoadAll<UnitConfig>("Scriptable Objects/Unit").ToList();
            _allTroopEffect = Resources.LoadAll<TroopEffect>("Scriptable Objects/Troop Effect").ToList();
            _allStatDecorator = Resources.LoadAll<StatDecorator>("Scriptable Objects/Stat Decorator").ToList();

            SetDeck(15);
            SetExtraDeck(15);
        }

        public TroopCard CreateTroopCard(Polyomino polyomino, TroopEffect troopEffect, StatDecorator statDecorator)
        {
            return new TroopCard(polyomino, troopEffect, statDecorator);
        }

        public TroopCard CreateRandomTroopCard()
        {
            Polyomino polyomino = Polyomino.GetRandomPolyomino();
            TroopEffect troopEffect = _allTroopEffect[Random.Range(0, _allTroopEffect.Count)];
            StatDecorator statDecorator = _allStatDecorator[Random.Range(0, _allStatDecorator.Count)];

            return new TroopCard(polyomino, troopEffect, statDecorator);
        }

        public void SetDeck(int deckAmount)
        {
            for (int i = 0; i < deckAmount; i++)
            {
                UnitConfig config = _unitPool[Random.Range(0, _unitPool.Count)];
                TroopCard troopCard = CreateRandomTroopCard();

                CardData card = new(config, troopCard);
                _deck.Add(card);
            }

            _deck.Shuffle();
        }

        public void SetDeck(Deck deck)
        {
            foreach(var card in deck.Decks)
            {
                if (card != null)
                {
                    _configs.Add(card.UnitConfig);
                }
            }
        }

        public void SetDeck(List<CardData> cardData)
        {
            _deck = cardData;
        }
        public void SetExtraDeck(int deckAmount)
        {
            for (int i = 0; i < deckAmount; i++)
            {
                UnitConfig config = _unitPool[Random.Range(0, _unitPool.Count)];
                TroopCard troopCard = CreateRandomTroopCard();
                CardData card = new(config, troopCard);

                _configs.Add(config);
                _troopCards.Add(troopCard);

                _extraDeck.Add(card);
            }

            _extraDeck.Shuffle();
        }


        public void SaveItemInUse(List<Item> items) {
            _itemInUse = items.ToList();
        }
    }
}
