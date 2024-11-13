using EnumTypes;
using System.Collections.Generic;

public interface IUnit
{
    public int MaxHP { get; }
    public int CurrentHP { get; }
    public int Attack { get; }
    public int SpellPower { get; }
    public int Range { get; }

    public int Defence { get; }
    public int SpellDefence { get; }

    public float Speed { get; }
    
    public Cell CurrentCell { get; set;  }
    public EnumTypes.CharacterTypes Owner { get; }

    public void TakeDamage(TurnContext turnContext, int damage, DamageTypes damageType = DamageTypes.True);
    public void TakeHeal(TurnContext turnContex, int amount);
}
