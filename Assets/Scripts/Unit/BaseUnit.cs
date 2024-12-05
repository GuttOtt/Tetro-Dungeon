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
    [SerializeField] private Cell _currentCell;
    [SerializeField] private int _id;
    public UnitConfig _config;
    public CharacterBlockConfig _characterBlockConfig;
    private UnitSPUMControl _spumControl;
    
    private float _speed;
    private float _actionCoolDown;

    private List<SynergyValuePair> _synergies;

    protected Projectile _projectilePrefab;

    [SerializeField] private CharacterTypes _owner;
    [SerializeField] private UnitDrawer _unitDrawer;
    #endregion

    #region Properties
    public string Name { get {
            if (_characterBlockConfig != null)
                return _characterBlockConfig.Name;
            else if (_config != null)
                return _config.Name;
            else
                Debug.LogError("config와 characterBlockConfig가 모두 비어 있습니다.");
            return null;
        }
    }

    #region Stats

    private int _maxHP, _currentHP, _attack, _defence, _currentAttack, _range, 
        _spellPower, _spellDefence, _unitTypeValue;
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
    private UnitSkill _defaultSkill;
    private List<UnitSkill> _activeSkills = new List<UnitSkill>();
    private float _skillChanceMultiplier;
    #endregion

    public int ID { get => _id; }
    public int UnitTypeValue { get => _unitTypeValue; }
    
    public Action OnDie { get; set; } //전투 중 유닛이 공격이나 효과에 의해 사망할 때 회출되는 이벤트.
    public Action OnDestroy { get; set; } //전투 중 사망하는 것을 포함해, '유닛의 GameObject가 제거될 때' 호출되는 이벤트. 전투 승리 시 남은 유닛의 처리 등에 의해서도 호출됨.
    public Cell CurrentCell { get => _currentCell; set => _currentCell = value; }
    public CharacterTypes Owner { get => _owner; set => _owner = value; }
    public int ConfigMaxHP {
        get {
            if (_characterBlockConfig != null)
                return _characterBlockConfig.Stat.MaxHP;
            else if (_config != null)
                return _config.MaxHP;
            else
                Debug.LogError("config와 characterBlockConfig가 모두 비어 있습니다.");
            return 0;

        } 
    }
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
            else if (_maxHP < _currentHP || ConfigMaxHP < _maxHP)
            {
                _unitDrawer.UpdateHP(_currentHP, new Color(0, 0.8f, 0));
            }
            else
            {
                _unitDrawer.UpdateHP(_currentHP, Color.black);
            }
        }
    }

    public List<SynergyValuePair> Synergies { get => _synergies; }
    public bool IsActionCoolDown { get => _actionCoolDown <= (1/Speed); }
    #endregion


    public virtual void Init(UnitSystem unitSystem, UnitConfig config, CharacterTypes owner, int id) { 
        //System
        _unitSystem = unitSystem;
        _id = id;

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

            //위치와 방향 설정
            spum.transform.localPosition = new Vector3(0, -0.2f, -1);
            if (owner == CharacterTypes.Player) {
                spum.transform.localRotation = new Quaternion(0, 180, 0, 0);
            }

            //임시 코드. Sprite Renderer가 전부 SPUM으로 대체되면 삭제해야 함.
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

        //Projectile
        if (config.Projectile != null) {
            _projectilePrefab = config.Projectile;
        }
    }

    public virtual void Init(UnitSystem unitSystem, CharacterBlockConfig config, CharacterTypes owner, int id) {
        //System
        _unitSystem = unitSystem;
        _id = id;

        //Config
        _characterBlockConfig = config;

        //Draw
        _unitDrawer = GetComponent<UnitDrawer>();
        _unitDrawer.Draw(config);

        //Spum
        if (config.SPUM_Prefabs != null) {
            if (_spumControl == null) {
                _spumControl = gameObject.AddComponent<UnitSPUMControl>();
            }
            SPUM_Prefabs spum = Instantiate(config.SPUM_Prefabs, gameObject.transform);
            _spumControl.SetSPUM(spum);

            //위치와 방향 설정
            spum.transform.localPosition = new Vector3(0, -0.2f, -1);
            if (owner == CharacterTypes.Player) {
                spum.transform.localRotation = new Quaternion(0, 180, 0, 0);
            }

            //임시 코드. Sprite Renderer가 전부 SPUM으로 대체되면 삭제해야 함.
            GetComponent<SpriteRenderer>().sprite = null;
        }

        //Stats
        Stat stat = config.Stat;
        _maxHP = stat.MaxHP;
        _currentHP = stat.MaxHP;
        _attack = stat.Attack;
        _currentAttack = stat.Attack;
        _spellPower = stat.SpellPower;
        _defence = stat.Defence;
        _spellDefence = stat.SpellDefence;
        _range = stat.Range;
        _speed = stat.Speed;

        //Skills
        _defaultSkill = SkillFactory.CreateSkill(config.DefaultSkill);
        foreach (SkillConfig skillConfig in config.Skills) {
            _activeSkills.Add(SkillFactory.CreateSkill(skillConfig));
        }

        _unitDrawer._healthBar.SetMaxHealth(MaxHP);

        _owner = owner;

        //Synergies
        //_synergies = config.Synergies;

        //Projectile
        /*
        if (config.Projectile != null) {
            _projectilePrefab = config.Projectile;
        }
        */
    }

    public void Die()
    {
        OnDie?.Invoke();
        OnDestroy?.Invoke();
        CurrentCell.UnitOut();
    }

    public void DestroySelf() {
        OnDestroy?.Invoke();
        CurrentCell.UnitOut();
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
            Debug.Log($"{Name} Attack");
        }
        else if (IsMovable(turnContext)) {
            Move(turnContext);
        }
        else {
            Debug.Log($"{Name} None Action");
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

        Debug.Log(movePath[1]);
        Debug.Log($"[{Name}, id: {_id}] Moved from ({CurrentCell.position.col}, {CurrentCell.position.row}) to ({movePath[1].position.col}, {movePath[1].position.row})");

        Cell moveCell = movePath[1];

        //유닛 이동
        if (moveCell.Unit != null) {
            Debug.LogError("이동하려는 칸에 이미 유닛이 있습니다!");
        }

        CurrentCell.UnitOut();
        moveCell.UnitIn(this);
        CurrentCell = movePath[1];

        //Transform 이동
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

        //closestOpponent를 공격할 수 있는 모든 셀 중 가장 가까운 경로를 만드는 셀을 구함
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

        if (shortestMovePath != null && shortestMovePath.Count <= 1) {
            Debug.LogError("shortestMovePath가 1임. 이런 경우는 존재해서는 안 됨. Board 클래스의 PathFinding 알고리즘을 확인해볼 것.");
        }

        //만약 공격 가능한 위치로 이동할 수 없다면, 그냥 가장 가까운 적을 향해 한 칸 이동할 수 있는지 계산
        if (shortestMovePath == null) {
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
        //이번 공격에 사용할 Active Skill을 선택
        UnitSkill skill = _defaultSkill;
        for (int i = 0; i < _activeSkills.Count; i++) {
            if (_activeSkills[i].CheckChance(_skillChanceMultiplier)) {
                skill = _activeSkills[i];
                break;
            }
        }

        //메인 타겟 설정
        BaseUnit mainTarget = GetAttackTarget(turnContext.Board) as BaseUnit;

        //공격 애니메이션 출력
        if (_spumControl != null) {
            _spumControl.PlayAttackAnimation(mainTarget);
            await UniTask.WaitUntil(() => _spumControl.IsCurrentAnimationTimePassed(0.85f));
        }

        //선택한 스킬을 발동
        Debug.Log($"[{Name}, id: {_id}]의 스킬 발동: {skill.SkillName}을 [{mainTarget.Name}, id: {mainTarget.ID}]에게 사용.");
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
            Debug.LogError($"There is no _projectilePrefab in {Name}. Please check the UnitConfig or Init method.");
        }

        Projectile projectile = Instantiate(_projectilePrefab);
        projectile.transform.position = transform.position;
        projectile.Init(target, onHit, speed);
    }

    protected void FireProjectile(Vector2 direction, Action<BaseUnit> onHit, float maxDistance, float speed = 7, int penetrateCount = 0) {
        if (_projectilePrefab == null) {
            Debug.LogError($"There is no _projectilePrefab in {Name}. Please check the UnitConfig or Init method.");
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


    public void Equip(EquipmentConfig equipmentConfig) {
        //Stats
        Stat equipmentStat = equipmentConfig.Stat;
        ChangeMaxHP(equipmentStat.MaxHP);
        ChangeAttack(equipmentStat.Attack);
        _spellPower += equipmentStat.SpellPower;
        _defence += equipmentStat.Defence;
        _spellDefence += equipmentStat.SpellDefence;
        _range += equipmentStat.Range;
        _speed += equipmentStat.Speed;
    }
}
