using EnumTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BaseUnit : MonoBehaviour, IUnit
{
    #region private members
    private UnitSystem _unitSystem;
    private Cell _currentCell;
    private UnitConfig _config;
    private int _maxHP, _maxMP, _currentHP, _currentMP, _attack, _currentAttack, _range, _unitTypeValue;
    private float _speed;
    private float _actionCoolDown;
    private List<SynergyTypes> _synergies;

    protected Projectile _projectilePrefab;

    [SerializeField] private CharacterTypes _owner;
    [SerializeField] private UnitDrawer _unitDrawer;
    #endregion

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int MaxMP { get => _maxMP; }
    public int Attack
    {
        get => _currentAttack;
        private set
        {
            _currentAttack = value;
            if (_currentAttack < _attack)
            {
                _unitDrawer.UpdateAttack(-_currentAttack, Color.red);
            }
            else if (_attack < _currentAttack)
            {
                _unitDrawer.UpdateAttack(_currentAttack, new Color(0, 0.8f, 0));
            }
            else
            {
                _unitDrawer.UpdateAttack(_currentAttack, Color.black);
            }
        }
    }
    public int Range { get => _range; }
    public float Speed { get => _speed; }

    public int UnitTypeValue { get => _unitTypeValue; }
    public Action OnDie { get; set; }
    public Cell CurrentCell { get => _currentCell; set => _currentCell = value; }
    public CharacterTypes Owner { get => _owner; set => _owner = value; }
    public int CurrentHP
    {
        get => _currentHP;
        private set
        {
            _currentHP = value;
            if (_currentHP < _maxHP)
            {
                _unitDrawer.UpdateHP(_currentHP, Color.red);
            }
            else if (_maxHP < _currentHP)
            {
                _unitDrawer.UpdateHP(_currentHP, new Color(0, 0.8f, 0));
            }
            else
            {
                _unitDrawer.UpdateHP(_currentHP, Color.black);
            }
        }
    }

    public List<SynergyTypes> Synergies { get => _synergies; }
    public bool IsActionCoolDown { get => _actionCoolDown <= (1/Speed); }
    #endregion


    public virtual void Init(UnitSystem unitSystem, UnitConfig config, CharacterTypes owner)
    {
        //System
        _unitSystem = unitSystem;

        //Config
        _config = config;

        //Draw
        _unitDrawer = GetComponent<UnitDrawer>();
        _unitDrawer.Draw(config);

        //Stats
        _maxHP = config.MaxHP;
        _currentHP = config.MaxHP;
        _maxMP = config.MaxMP;
        _currentMP = config.MaxMP;
        _attack = config.Attack;
        _currentAttack = config.Attack;
        _range = config.Range;
        _speed = config.Speed;

        _unitTypeValue = config.UnitTypeValue;
        _unitDrawer._healthBar.SetMaxHealth(MaxHP);

        _owner = owner;

        //Synergies
        _synergies = config.Synergies;

        //Projectile
        if (config.Projectile != null) {
            _projectilePrefab = config.Projectile;
        }
    }

    public void Die()
    {
        OnDie();
        _unitSystem.DestroyUnit(this);
    }

    public virtual void Die(TurnContext turnContext)
    {
        Die();
    }

    public void Highlight()
    {
        if (_unitDrawer != null)
        {
            _unitDrawer.Highlight();
        }
        else
        {
            Debug.LogWarning("UnitDrawer is missing or destroyed.");
        }
    }

    public void Unhighlight()
    {
        //_unitDrawer.Unhighlight();
        if (Owner == CharacterTypes.Player)
            _unitDrawer.ChangeColor(Color.white);
        else
            _unitDrawer.ChangeColor(new Color(1, 0.5f, 0.5f));
    }

    #region Unit Action Pattern
    public void Act(TurnContext turnContext) {
        if (IsAttackable(turnContext)) {
            AttackAnimation(turnContext);
            AttackAction(turnContext);
            Debug.Log($"{_config.name} Attack");
        }
        else if (IsMovable(turnContext)) {
            Move(turnContext);
        }
        else {
            Debug.Log($"{_config.name} None Action");
        }

        ResetActionCoolDown();
    }

    #region Move
    public virtual bool IsMovable(TurnContext turnContext) {
        return GetMovePath(turnContext.Board) != null;
    }

    public virtual void Move(TurnContext turnContext)
    {
        List<Cell> movePath = GetMovePath(turnContext.Board);

        Debug.Log($"{_config.name} Moved from ({CurrentCell.position.col}, {CurrentCell.position.row}) to ({movePath[1].position.col}, {movePath[1].position.row})");

        //유닛 이동
        CurrentCell.UnitOut();
        movePath[1].UnitIn(this);
        CurrentCell = movePath[1];

    }

    protected List<Cell> GetMovePath(Board board) {
        IUnit closestOpponent = board.GetClosestUnit(CurrentCell, Owner.Opponent(), 100);
        if (closestOpponent == null) {
            return null;
        }

        //closestOpponent를 공격할 수 있는 모든 셀 중 가장 가까운 경로를 만드는 셀을 구함
        List<Cell> attackableCells = board.GetCellsInRange(closestOpponent.CurrentCell, 1, Range);

        List<Cell> shortestMovePath = null;
        int shortestDistance = int.MaxValue;

        foreach (Cell cell in attackableCells) {
            List<Cell> movePath = board.PathFinding(CurrentCell, cell);

            if (2 <= movePath.Count && movePath.Count < shortestDistance) {
                shortestMovePath = movePath;
                shortestDistance = movePath.Count;
            }
        }

        /* Debug.Logs
        if (shortestMovePath == null) {
            Debug.Log($"No path to attackable Cells. Unit Name: {_config.name}, attackableCells.Count: {attackableCells.Count}, closestOpponent: {(closestOpponent as BaseUnit)._config.name}");
        }
        else if (shortestMovePath.Count <= 1) {
            Debug.Log($"shortestMovePath.Count = {shortestMovePath.Count}. Owner : {Owner}, Unit Name: {_config.name}, attackableCells.Count: {attackableCells.Count}, closestOpponent: {(closestOpponent as BaseUnit)._config.name}");
        }
        */

        //만약 공격 가능한 위치로 이동할 수 없다면, 그냥 가장 가까운 적을 향해 한 칸 이동할 수 있는지 계산
        if (shortestMovePath == null || shortestMovePath.Count <= 1) {
            int forwardOffset = Owner == CharacterTypes.Player ? 1 : -1;
            Cell forwardCell = board.GetCell(CurrentCell.position.col + forwardOffset, CurrentCell.position.row);

            if (forwardCell != null && forwardCell.Unit == null) {
                shortestMovePath = new List<Cell> { CurrentCell, forwardCell };
            }
        }

        return shortestMovePath;
    }
    #endregion

    #region Attack
    public virtual bool IsAttackable(TurnContext turnContext)
    {
        IUnit targetUnit = GetAttackTarget(turnContext.Board);
        if (targetUnit != null && targetUnit as BaseUnit != null)
            return true;
        else
            return false;
    }

    public virtual void AttackAction(TurnContext turnContext)
    {
        IUnit attackTarget = GetAttackTarget(turnContext.Board);

        attackTarget.TakeDamage(turnContext, Attack);
    }

    protected IUnit GetAttackTarget(Board board)
    {
        Cell currentCell = CurrentCell;

        IUnit attackTarget = board.GetClosestUnit(currentCell, Owner.Opponent(), Range);

        return attackTarget;
    }

    //애니메이션이나 transform 움직임은 별도의 클래스로 이동하는 게 좋을 수 있음
    protected void AttackAnimation(TurnContext turnContext) {
        IUnit target = GetAttackTarget(turnContext.Board);

        if (target == null || target as BaseUnit == null) {
            return;
        }

        Vector3 targetPos = (target as BaseUnit).transform.position; 
        Vector3 moveVector = transform.position + (targetPos - transform.position).normalized * 0.3f;

        transform.DOMove(moveVector, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    protected void FireProjectile(BaseUnit target, Action<BaseUnit> onHit, float speed = 7) {
        if (_projectilePrefab == null) {
            Debug.LogError($"There is no _projectilePrefab in {_config.name}. Please check the UnitConfig or Init method.");
        }

        Projectile projectile = Instantiate(_projectilePrefab);
        projectile.transform.position = transform.position;
        projectile.Init(target, onHit, speed);
    }

    protected void FireProjectile(Vector2 direction, Action<BaseUnit> onHit, float maxDistance, float speed = 7, int penetrateCount = 0) {
        if (_projectilePrefab == null) {
            Debug.LogError($"There is no _projectilePrefab in {_config.name}. Please check the UnitConfig or Init method.");
        }

        Projectile projectile = Instantiate(_projectilePrefab);
        projectile.transform.position = transform.position;
        projectile.Init(direction, onHit, maxDistance, speed, penetrateCount);
    }
    #endregion

    #region Stat
    public void SetAttack(int value) => Attack = value;
    public void SetCurrentHP(int value) => CurrentHP = value;

    public void ChangeAttack(int value) => Attack += value;
    public void ChangeCurrentHP(int value) => CurrentHP += value;

    public void ChangeMaxHP(int value) => _maxHP += value;
    #endregion

    public virtual void AttackedBy(TurnContext turnContext, int damage, IUnit attacker)
    {
        TakeDamage(turnContext, damage);
    }

    public virtual void TakeDamage(TurnContext turnContext, int damage)
    {
        _unitDrawer.DisplayDamageText(damage);
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die(turnContext);
        }
    }

    public virtual void TakeHeal(TurnContext turnContext, int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }



    #endregion

    public void ActionCoolDown(float time) {
        _actionCoolDown += time;
    }

    public void ResetActionCoolDown() {
        _actionCoolDown = 0;
    }

}
