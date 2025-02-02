using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour {
    [SerializeField] private BaseUnit _target;
    [SerializeField] private bool _isTargetBased;
    private Vector2 _direction;
    [SerializeField] private float _speed = 7;
    private int _penetrateCount = 0;

    private float _maxDistance = 1;
    private float _flyDistance = 0;
    private Damage _damage;
    private DamageTypes _damageType;
    private TurnContext _turnContext;
    private Action<TurnContext, BaseUnit, Damage> _onHit;

    private List<BaseUnit> _hitUnits = new List<BaseUnit>();

    public void Init(TurnContext turnContext, BaseUnit target, Damage damage, Action<TurnContext, BaseUnit, Damage> onHit, float speed = 7) {
        _turnContext = turnContext;
        _target = target;
        _speed = speed;
        _onHit = onHit;
        _damage = damage;
        _isTargetBased = true;
    }

    public void Init(TurnContext turnContext, Vector2 direction, Action<BaseUnit> onHit, float maxDistance, float speed = 7, int penetrateCount = 0) {
        
        _target = null;
        _direction = direction;
        _speed = speed;
        _penetrateCount = penetrateCount;
        _maxDistance = maxDistance;

        _isTargetBased = false;
        //SetRotation(direction);
    }

    private void Update() {
        if (_isTargetBased && _target == null)
            Destroy(gameObject);

        FlyToTarget();
        FlyToDirection();
        CheckHit();

        if (_maxDistance <= _flyDistance) {
            Destroy(gameObject);
        }
    }

    //Target을 향해 날아가는 방식
    private void FlyToTarget() {
        if (_target == null || _target.transform == null) return;

        Transform targetTransform = _target.transform;

        Vector2 direction = targetTransform.position - transform.position;
        Vector3 moveVector = direction.normalized * _speed * Time.deltaTime;

        transform.position += moveVector;

        SetRotation(direction);
    }

    //Target 없이 일정한 방향을 향해 날아가는 방식
    private void FlyToDirection() {
        if (_direction == null || _target != null) return;

        Vector3 moveVector = _direction.normalized * _speed * Time.deltaTime;

        transform.position += moveVector;

        _flyDistance += moveVector.magnitude;
    }

    private void CheckHit() {
        BaseUnit hitUnit = Utils.Pick<BaseUnit>(transform.position);

        if (hitUnit == null) return;

        if (_target != null) {

            if (hitUnit == _target) {
                Damage dealtDamage = hitUnit.TakeDamage(_turnContext, _damage);
                _onHit?.Invoke(_turnContext, hitUnit, dealtDamage);
                Destroy(gameObject);
            }
            else {
                return;
            }
        }
        else {
            if (_hitUnits.Contains(hitUnit)) {
                return;
            }

            _hitUnits.Add(hitUnit);

            _penetrateCount--;
            if (_penetrateCount < 0) {
                Debug.Log("Penetration ends. destroy projectile.");
                Destroy(gameObject);
            }
        }
    }

    private void SetRotation(Vector2 direction) {
        transform.right = direction;

    }
}
