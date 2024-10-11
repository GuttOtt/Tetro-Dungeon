namespace EnumTypes
{
    public enum CharacterTypes
    {
        None, Player, Enemy,

        
    }
    public static class CharacterTypeExtensions
    {
        public static CharacterTypes Opponent(this CharacterTypes characterType) {
            switch (characterType) {
                case CharacterTypes.Player:
                    return CharacterTypes.Enemy;
                case CharacterTypes.Enemy:
                    return CharacterTypes.Player;
                default:
                    return characterType;
            }
        }
    }
    public enum CardPileTypes
    {
        Deck, Hand, Discard,
    }

    public enum SynergyTypes {
        None = 0,Dragon=1, Gunner=2, Plant=3
    }

    public enum DamageTypes {
        Attack, Spell, True
    }
}
