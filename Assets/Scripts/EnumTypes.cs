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
        None = 0,Magician = 1, Swordman =2, Fire = 3, Dark = 4, Light =5, Shooter = 6,
    }

    public enum DamageTypes {
        Attack, Spell, True
    }

    public enum UnitEventTypes {
        OnDying = 0, OnAttacking = 1, OnAttacked = 2, OnDamageTaken = 3, OnEverySeconds = 4, OnBattleStart = 5,
    }

    public enum TargetTypes {
        AttackTarget = 0, ClosestAlly = 1, AllAllys = 2,
    }
}
