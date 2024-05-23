public interface IUnit
{
    public int MaxHP { get; }
    public int MaxMP { get; }
    public int Attack { get; }
    public int Range { get; }
    public Cell CurrentCell { get; }
    public EnumTypes.CharacterTypes Owner { get; }
}
