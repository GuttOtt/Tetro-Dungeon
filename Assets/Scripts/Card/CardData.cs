using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    private UnitConfig _unitConfig;
    private TroopCard _troopCard;

    public CardData(UnitConfig unitConfig, TroopCard troopCard)
    {
        _unitConfig = unitConfig;
        _troopCard = troopCard;
    }

    public TroopCard TroopCard { get => _troopCard; }
    public Polyomino Polyomino { get => _troopCard.Polyomino; }
    public UnitConfig UnitConfig { get => _unitConfig; }

    public ICard DeepCopy()
    {
        throw new System.NotImplementedException();
    }


}
