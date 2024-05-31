using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSystem : MonoBehaviour {
    #region private members
    private IGameManager _gameManager;

    //Phase 제어 관련 변수들
    private PhaseContext _phaseContext;
    private IPhase _standbyPhase, _mainPhase, _battlePhase, _endPhase;

    //참조 시스템들
    private CardSystem _cardSystem;
    private BattleSystem _battleSystem;
    private EnemySystem _enemySystem;
    #endregion

    public CardSystem CardSystem { get => _cardSystem; }
    public BattleSystem BattleSystem { get => _battleSystem; }
    public EnemySystem EnemySystem { get => _enemySystem; }


    private void Start() {
        _gameManager = transform.parent.GetComponent<IGameManager>();
        _cardSystem = _gameManager.GetSystem<CardSystem>();
        _battleSystem = _gameManager.GetSystem<BattleSystem>();
        _enemySystem = _gameManager.GetSystem<EnemySystem>();

        _phaseContext = new PhaseContext(this);

        _standbyPhase = new StandbyPhase();
        _mainPhase = new MainPhase();
        _battlePhase = new BattlePhase();
        _endPhase = new EndPhase();
    }

    public void ToStandbyPhase() {
        _phaseContext.Transit(_standbyPhase);
    }

    public void ToMainPhase() {
        _phaseContext.Transit(_mainPhase);
    }

    public void ToBattlePhase() {
        _phaseContext.Transit(_battlePhase);
    }

    public void ToEndPhase() {
        _phaseContext.Transit(_endPhase);
    }
}

public class PhaseContext {
    public IPhase CurrentPhase { get; set; }

    private readonly PhaseSystem _phaseSystem;

    public PhaseContext(PhaseSystem phaseSystem) {
        _phaseSystem = phaseSystem;
    }

    public void Transit(IPhase phase) {
        CurrentPhase?.ExitPhase(_phaseSystem);
        CurrentPhase = phase;
        CurrentPhase.EnterPhase(_phaseSystem);
    }
}

public class StandbyPhase : IPhase {
    public void EnterPhase(PhaseSystem phaseSystem) {
        //Draw
        CardSystem cardSystem = phaseSystem.CardSystem;
        cardSystem.DrawCard();

        //Decide Enemy Units
        EnemySystem enemySystem = phaseSystem.EnemySystem;
        enemySystem.DecideUnitList();
    }

    public void ExitPhase(PhaseSystem phaseSystem) {
    }
}

public class MainPhase : IPhase {
    public void EnterPhase(PhaseSystem phaseSystem) {
        //Card Input On
        phaseSystem.CardSystem.SetInputOn();
    }

    public void ExitPhase(PhaseSystem phaseSystem) {
        //Card Input Off
        phaseSystem.CardSystem.SetInputOff();
    }
}

public class BattlePhase : IPhase {
    public void EnterPhase(PhaseSystem phaseSystem) {
        //Place Enemy Units
        phaseSystem.EnemySystem.PlaceUnit();

        //Start Battle
        phaseSystem.BattleSystem.StartBattle();
    }

    public void ExitPhase(PhaseSystem phaseSystem) {
        //End Battle
        //Damage to life
    }
}

public class EndPhase : IPhase {
    public void EnterPhase(PhaseSystem phaseSystem) {

    }

    public void ExitPhase(PhaseSystem phaseSystem) {
        //Card Input Off
    }
}