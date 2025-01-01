using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableStatusConfig : StatusConfig
{
    [SerializeField] private int _maxCount;

    public int MaxCount { get => _maxCount; }
}

public class CountableStatus : Status {
    [SerializeField] private int _maxCount;
    private int _currentCount = 0;

    public int MaxCount { get => _maxCount; }

    public CountableStatus(CountableStatusConfig config) : base(config) {
        _maxCount = config.MaxCount;
    }


    public override void ApplyTo(BaseUnit unit) {
        if (HasAleady(unit)) {
            CountableStatus status = unit.GetStatus(this.Name) as CountableStatus;
            status.CountUp();
        }
        else {
            unit.GrantStatus(this);
            CountUp();
        }
    }

    public override void RemoveFrom(BaseUnit unit)
    {
        throw new System.NotImplementedException();
    }


    public void CountUp() {
        _currentCount++;

        if (_maxCount <= _currentCount){
            _currentCount = 0;
        }
    }

    private bool HasAleady(BaseUnit unit) {
        return unit.GetStatus(this.Name) != null;
    }
}
