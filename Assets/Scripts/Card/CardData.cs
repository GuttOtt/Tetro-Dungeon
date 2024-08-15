using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    private UnitConfig _unitConfig;
    private BlockCard _blockCard;

    public CardData(UnitConfig unitConfig, BlockCard troopCard)
    {
        _unitConfig = unitConfig;
        _blockCard = troopCard;
    }

    public BlockCard BlockCard { get => _blockCard; }
    public Polyomino Polyomino { get => _blockCard.Polyomino; }
    public UnitConfig UnitConfig { get => _unitConfig; }

    public ICard DeepCopy()
    {
        throw new System.NotImplementedException();
    }


}
