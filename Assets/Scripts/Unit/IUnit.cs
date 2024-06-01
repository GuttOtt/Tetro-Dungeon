public interface IUnit
{
    public int MaxHP { get; }
    public int MaxMP { get; }
    public int Attack { get; }
    public int Range { get; }
    public int CurrentHP { get; }    
    public Cell CurrentCell { get; set;  }
    public EnumTypes.CharacterTypes Owner { get; }
    public void TakeDamage(int damage);
    public void Die();
}
