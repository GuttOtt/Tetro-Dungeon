using EnumTypes;
using System.Collections.Generic;

public interface IUnit
{
    public int MaxHP { get; }
    public int MaxMP { get; }
    public int Attack { get; }
    public int Range { get; }
    public int CurrentHP { get; }    
    
    public Cell CurrentCell { get; set;  }
    public EnumTypes.CharacterTypes Owner { get; }

    public List<SynergyTypes> Synergies { get; }

    public void TakeDamage(TurnContext turnContext, int damage);
    public void TakeHeal(TurnContext turnContex, int amount);
}
