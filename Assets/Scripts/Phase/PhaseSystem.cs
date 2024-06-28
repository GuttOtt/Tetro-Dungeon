using Cysharp.Threading.Tasks;
using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    //페이즈 표시 관련
    [SerializeField] private TMP_Text _phaseText;

    //현재 공격 턴
    private CharacterTypes _attackTurn = CharacterTypes.Player;
    #endregion

    public CardSystem CardSystem { get => _cardSystem; }
    public BattleSystem BattleSystem { get => _battleSystem; }
    public EnemySystem EnemySystem { get => _enemySystem; }
    public CharacterTypes AttackTurn { get => _attackTurn; }

    private void Awake() {
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

    public async void ToStandbyPhase() {
        _phaseText.text = "Standby Phase";
        _phaseContext.Transit(_standbyPhase);

        await UniTask.WaitForSeconds(2);

        ToMainPhase();
    }

    public void ToMainPhase() {
        _phaseText.text = "Main Phase";
        _phaseContext.Transit(_mainPhase);
    }

    public void ToBattlePhase() {
        _phaseText.text = "Battle Phase";
        _phaseContext.Transit(_battlePhase);
    }

    public void ToEndPhase() {
        _phaseText.text = "End Phase";
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
        cardSystem.DrawCard(5);

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

        //Discard all hands
        phaseSystem.CardSystem.DiscardAllHand();
    }
}

public class BattlePhase : IPhase {
    public void EnterPhase(PhaseSystem phaseSystem) {
        //Place Enemy Units
        phaseSystem.EnemySystem.PlaceUnit();

        //Start Battle
        phaseSystem.BattleSystem.StartBattle().Forget();
    }

    public void ExitPhase(PhaseSystem phaseSystem) {
        //End Battle
        //Damage to life
    }
}

public class EndPhase : IPhase {
    public void EnterPhase(PhaseSystem phaseSystem) {
        phaseSystem.ToStandbyPhase();
    }

    public void ExitPhase(PhaseSystem phaseSystem) {
        //Card Input Off
    }
}