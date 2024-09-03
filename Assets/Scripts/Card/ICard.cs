using System;

public interface ICard
{
    public BlockCard BlockCard { get; }
    public Polyomino Polyomino { get; }
    public UnitConfig UnitConfig { get; }
    public int Cost { get; }

    public ICard DeepCopy();
}
