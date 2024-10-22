using EnumTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;

public class BaseUnit : MonoBehaviour, IUnit
{
    #region private members
    private UnitSystem _unitSystem;
    [SerializeField]
    private Cell _currentCell;
    public UnitConfig _config;
    private UnitSPUMControl _spumControl;
    private int _maxHP, _currentHP, _attack, _defence, _currentAttack, _range, _spellPower, _spellDefence, _unitTypeValue;
    private float _speed;
    private float _actionCoolDown;
    private List<SynergyTypes> _synergies;

    protected Projectile _projectilePrefab;

    [SerializeField] private CharacterTypes _owner;
    [SerializeField] private UnitDrawer _unitDrawer;
    #endregion

    #region Properties
    #region Stats
    public int MaxHP { get => _maxHP; }
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
    public int SpellPower { get => _spellPower; }
    public int Range { get => _range; }
    public float Speed { get => _speed; }
    public int Defence { get => _defence; }
    public int SpellDefence { get => _spellDefence; }
    public float AttackDamageReductionRate { get => 1f / (1 + Defence / 10f); }
    public float SpellDamageReductionRate { get => 1f / (1 + SpellDefence / 10f); }
    #endregion
    #region Skills
    private ActiveSkill _defaultSkill;
    private List<ActiveSkill> _activeSkills;
    private float _skillChanceMultiplier;
    #endregion

    public int UnitTypeValue { get => _unitTypeValue; }
    
    public Action OnDie { get; set; } //���� �� ������ �����̳� ȿ���� ���� ����� �� ȸ��Ǵ� �̺�Ʈ.
    public Action OnDestroy { get; set; } //���� �� ����ϴ� ���� ������, '������ GameObject�� ���ŵ� ��' ȣ��Ǵ� �̺�Ʈ. ���� �¸� �� ���� ������ ó�� � ���ؼ��� ȣ���.
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
            else if (_maxHP < _currentHP || _config.MaxHP < _maxHP)
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

        //Spum
        if (_config.SPU_Prefabs != null) {
            if (_spumControl == null) {
                _spumControl = gameObject.AddComponent<UnitSPUMControl>();
            }
            SPUM_Prefabs spum = Instantiate(_config.SPU_Prefabs, gameObject.transform);
            _spumControl.SetSPUM(spum);

            //��ġ�� ���� ����
            spum.transform.localPosition = new Vector3(0, -0.2f, -1);
            if (owner == CharacterTypes.Player) {
                spum.transform.localRotation = new Quaternion(0, 180, 0, 0);
            }

            //�ӽ� �ڵ�. Sprite Renderer�� ���� SPUM���� ��ü�Ǹ� �����ؾ� ��.
            GetComponent<SpriteRenderer>().sprite = null;
        }

        //Stats
        _maxHP = config.MaxHP;
        _currentHP = config.MaxHP;
        _attack = config.Attack;
        _currentAttack = config.Attack;
        _spellPower = config.SpellPower;
        _defence = config.Defence;
        _spellDefence = config.SpellDefence;
        _range = config.Range;
        _speed = config.Speed;

        //Skills
        _defaultSkill = config.DefaultSkill;
        _activeSkills = config.ActiveSkills.ToList();

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
        OnDie?.Invoke();
        OnDestroy?.Invoke();
    }

    public void DestroySelf() {
        OnDestroy.Invoke();
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

        Cell moveCell = movePath[1];

        //���� �̵�
        if (moveCell.Unit != null) {
            Debug.LogError("�̵��Ϸ��� ĭ�� �̹� ������ �ֽ��ϴ�!");
        }

        CurrentCell.UnitOut();
        moveCell.UnitIn(this);
        CurrentCell = movePath[1];

        //Transform �̵�
        var tween = transform.DOMove(moveCell.transform.position, Speed * 0.8f).SetEase(Ease.OutBack, 2f);
        tween.OnStart(() =>
            _spumControl?.ChangeState(PlayerState.MOVE)
        );
        tween.OnComplete(() =>
            _spumControl?.ChangeState(PlayerState.IDLE)
        );
    }

    protected List<Cell> GetMovePath(Board board) {
        IUnit closestOpponent = board.GetClosestUnit(CurrentCell, Owner.Opponent(), 100);
        if (closestOpponent == null) {
            return null;
        }

        //closestOpponent�� ������ �� �ִ� ��� �� �� ���� ����� ��θ� ����� ���� ����
        List<Cell> attackableCells = board.GetEmptyCellsInRange(closestOpponent.CurrentCell, 1, Range);

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

        //���� ���� ������ ��ġ�� �̵��� �� ���ٸ�, �׳� ���� ����� ���� ���� �� ĭ �̵��� �� �ִ��� ���
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

    public async virtual void AttackAction(TurnContext turnContext)
    {
        //�̹� ���ݿ� ����� Active Skill�� ����
        ActiveSkill skill = _defaultSkill;
        for (int i = 0; i < _activeSkills.Count; i++) {
            if (_activeSkills[i].CheckChance(_skillChanceMultiplier)) {
                skill = _activeSkills[i];
                break;
            }
        }

        //���� Ÿ�� ����
        BaseUnit mainTarget = GetAttackTarget(turnContext.Board) as BaseUnit;

        //���� �ִϸ��̼� ���
        if (_spumControl != null) {
            _spumControl.PlayAttackAnimation(mainTarget);
            await UniTask.WaitUntil(() => _spumControl.IsCurrentAnimationTimePassed(0.85f));
        }

        //������ ��ų�� �ߵ�
        Debug.Log($"{_config.name}�� ��ų �ߵ�: {skill.name}�� {mainTarget._config.name}���� ���.");
        skill.Activate(turnContext, this, mainTarget);
    }

    protected IUnit GetAttackTarget(Board board)
    {
        Cell currentCell = CurrentCell;

        IUnit attackTarget = board.GetClosestUnit(currentCell, Owner.Opponent(), Range);

        return attackTarget;
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
    public void SetMaxHP(int value) {
        float ratio = CurrentHP / MaxHP;

        _maxHP = value;
        CurrentHP = (int) (value * ratio);
    }

    public void ChangeAttack(int value) => Attack += value;
    public void ChangeCurrentHP(int value) => CurrentHP += value;

    public void ChangeMaxHP(int value) {
        float ratio = CurrentHP / MaxHP;
        _maxHP += value;
        CurrentHP = (int)(_maxHP * ratio);
    }
    public void ChangeSpeed(float value) => _speed += value;
    #endregion

    public virtual void AttackedBy(TurnContext turnContext, int damage, IUnit attacker)
    {
        TakeDamage(turnContext, damage);
    }

    public virtual void TakeDamage(TurnContext turnContext, int damage, DamageTypes damageType = DamageTypes.True)
    {
        int reducedDamage = damage;

        switch (damageType) {
            case DamageTypes.Attack:
                reducedDamage -= (int)(damage * AttackDamageReductionRate);
                break;
            case DamageTypes.Spell:
                reducedDamage -= (int)(damage * SpellDamageReductionRate);
                break;
            case DamageTypes.True:
                reducedDamage -= 0;
                break;
        }

        _unitDrawer.DisplayDamageText(damage, damageType);
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
