using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSystem : MonoBehaviour {
    #region private members
    private IGameManager _gameManager;
    [SerializeField] private BaseUnit _unitPrefab;
    #endregion

    private void Awake() {
        _gameManager = transform.parent.GetComponent<GameManager>();
    }

    public IUnit CreateUnit(UnitConfig unitConfig) {
        BaseUnit unit = Instantiate(_unitPrefab);
        unit.Init(unitConfig);

        return unit;
    }
}
