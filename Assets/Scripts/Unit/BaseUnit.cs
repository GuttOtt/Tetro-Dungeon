using System;
using UnityEngine;
using EnumTypes;
using System.Buffers;
using Unity.Mathematics;

public class BaseUnit : MonoBehaviour, IUnit
{
    #region private members
    private UnitSystem _unitSystem;
    private Cell _currentCell;
    private UnitConfig _config;
    private int _maxHP, _maxMP, _currentHP, _currentMP, _attack, _range;
    [SerializeField] private CharacterTypes _owner;
    [SerializeField] private UnitDrawer _unitDrawer;
    #endregion

    #region Properties
    public int MaxHP { get => _maxHP; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }   
    public int Range { get => _range; }
    public Action OnDie { get; set; }
    public Cell CurrentCell { get => _currentCell; set => _currentCell = value; }
    public CharacterTypes Owner { get => _owner; set => _owner = value; }
    public int CurrentHP { 
        get => _currentHP;
        private set {
            _currentHP = value;
            if (_currentHP < _maxHP) {
                _unitDrawer.UpdateHP(_currentHP, Color.red);
            }
            else if (_maxHP < _currentHP) {
                _unitDrawer.UpdateHP(_currentHP, Color.green);
            }
            else {
                _unitDrawer.UpdateHP(_currentHP, Color.black);
            }
        }
    }
    #endregion


    public void Init(UnitSystem unitSystem, UnitConfig config, CharacterTypes owner) {
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
        _range = config.Range;

        _owner = owner;

    }

    public void TakeDamage(int damage) {
        CurrentHP -= damage;
    }

    public void Die() {
        OnDie();
        _unitSystem.DestroyUnit(this);
    }

    public void Highlight() {
        _unitDrawer.Highlight();
    }

    public void Unhighlight() {
        _unitDrawer.Unhighlight();
    }

    #region Unit Action Pattern

    #region Move
    public virtual bool IsMoveable(TurnContext turnContext) {
        CharacterTypes moveTurn = turnContext.MoveTurn;

        if (Owner != moveTurn) {
            return false;
        }

        //유닛의 앞쪽 셀을 가져옴
        Cell forwardCell = GetForwardCell(turnContext.Board);

        //forwardCell에 이미 유닛이 있거나, forwardCell이 존재하지 않는다면 false
        if (forwardCell == null || forwardCell.Unit != null)
            return false;

        return true;
    }

    public virtual void Move(TurnContext turnContext) {
        Cell forwardCell = GetForwardCell(turnContext.Board);

        //유닛 이동
        CurrentCell.UnitOut();
        forwardCell.UnitIn(this);
        CurrentCell = forwardCell;
    }

    protected Cell GetForwardCell(Board board) {
        Cell currentCell = CurrentCell;

        // 전방으로 한 칸의 위치 계산
        int forwardOffset = Owner == CharacterTypes.Player ? 1 : -1;
        int targetColumn = currentCell.position.col + forwardOffset;

        //유닛의 앞쪽 셀을 가져옴
        Cell forwardCell = board.GetCell(targetColumn, currentCell.position.row);

        return forwardCell;
    }
    #endregion

    #region Attack
    public virtual bool IsAttackable(TurnContext turnContext) {
        IUnit targetUnit = GetAttackTarget(turnContext.Board);
        if (targetUnit != null)
            return true;
        else
            return false;
    }

    public virtual void AttackAction(TurnContext turnContext) {
        IUnit attackTarget = GetAttackTarget(turnContext.Board);

        attackTarget.TakeDamage(Attack);
    }

    protected IUnit GetAttackTarget(Board board) {
        int range = Range;
        int forwardOffset = Owner == CharacterTypes.Player ? 1 : -1;
        Cell currentCell = CurrentCell;
        int originCol = currentCell.position.col;
        int originRow = currentCell.position.row;

        //가까운 유닛을 우선으로 공격
        for (int i = 1; i <= range; i++) {
            Cell targetCell = board.GetCell(originCol + forwardOffset * i, originRow);
            IUnit targetUnit = targetCell.Unit;

            if (targetUnit != null && Owner != targetUnit.Owner) {
                return targetUnit;
            }
        }

        return null;
    }
    #endregion

    public virtual void AttackedBy(TurnContext turnContext, int damage, IUnit attacker) {
        TakeDamage(turnContext, damage);
    }

    public virtual void TakeDamage(TurnContext turnContext, int damage) {
        CurrentHP -= damage;
    }

    #endregion
}
