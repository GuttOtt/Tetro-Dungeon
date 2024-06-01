using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    #region private members
    [SerializeField]
    private CardSystem _cardSystem;

    [SerializeField]
    private UnitSystem _unitSystem;

    [SerializeField]
    private Board _board;

    [SerializeField]
    private PhaseSystem _phaseSystem;

    [SerializeField]
    private BattleSystem _battleSystem;

    [SerializeField]
    private EnemySystem _enemySystem;
    #endregion

    private void Start() {
        StartBattleScene();
    }

    private void StartBattleScene() {
        _cardSystem.SetDeck(15);
        _board.Init();
        _phaseSystem.ToStandbyPhase();
    }

    public T GetSystem<T>() where T : class {
        if (typeof(T) == typeof(CardSystem)) {
            return _cardSystem as T;
        }
        else if (typeof(T) == typeof(UnitSystem)) {
            return _unitSystem as T;
        }
        else if (typeof(T) == typeof(Board)) {
            return _board as T;
        }
        else if (typeof(T) == typeof(PhaseSystem)) {
            return _phaseSystem as T;
        }
        else if(typeof(T) == typeof(BattleSystem)) {
            return _battleSystem as T;
        }
        else if (typeof(T) == typeof(EnemySystem)) {
            return _enemySystem as T;
        }
        else {
            Debug.LogError($"System of type {typeof(T)} is not supported.");
            return null;
        }
    }
}
