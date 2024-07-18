using System;

public interface ICard
{
    public TroopCard TroopCard { get; }
    public Polyomino Polyomino { get; }
    public UnitConfig UnitConfig { get; }

    public ICard DeepCopy();
}
