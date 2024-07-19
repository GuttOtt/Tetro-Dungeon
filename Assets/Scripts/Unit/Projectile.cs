using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour {
    [SerializeField]
    private float hitDistance;

    private Action<BaseUnit> _onHit;
    private BaseUnit _target;
    private Vector2 _direction;
    private float _speed = 7;
    private int _penetrateCount = 0;

    private float _maxDistance = 1;
    private float _flyDistance = 0;

    private List<BaseUnit> _hitUnits = new List<BaseUnit>();

    public void Init(BaseUnit target, Action<BaseUnit> onHit, float speed = 7) {
        _target = target;
        _speed = speed;
        _onHit = onHit;
    }

    public void Init(Vector2 direction, Action<BaseUnit> onHit, float maxDistance, float speed = 7, int penetrateCount = 0) {
        _direction = direction;
        _speed = speed;
        _onHit = onHit;
        _penetrateCount = penetrateCount;

    }

    private void Update() {
        if (_target == null || _target.gameObject == null)
            Destroy(gameObject);

        FlyToTarget();
        FlyToDirection();
        CheckHit();

        if (_maxDistance <= _flyDistance) {
            Destroy(gameObject);
        }
    }

    //Target�� ������ ���ư��� ���
    private void FlyToTarget() {
        if (_target == null || _target.transform == null) return;

        Transform targetTransform = _target.transform;

        Vector2 direction = targetTransform.position - transform.position;
        Vector3 moveVector = direction.normalized * _speed * Time.deltaTime;

        transform.position += moveVector;
    }

    //Target ���� ������ ������ ���� ���������� ���ư��� ���
    private void FlyToDirection() {
        if (_direction == null || _target != null) return;

        Vector3 moveVector = _direction.normalized * _speed * Time.deltaTime;

        transform.position += moveVector;

        _flyDistance += moveVector.magnitude;
    }

    private void CheckHit() {
        BaseUnit hitUnit = Utils.Pick<BaseUnit>(transform.position);

        if (hitUnit == null) return;

        //Ÿ���� �ִ� ���
        if (_target != null) {
            Debug.Log("Col to the target");

            //��ġ�� ���, OnHit ���� �� ����
            if (hitUnit == _target) {
                _onHit.Invoke(_target);
                Destroy(gameObject);
            }
            //��ġ���� ������ ����
            else {
                return;
            }
        }
        //Ÿ�� ���� ��� (������ ������ ���ư��� ���)
        else {
            if (hitUnit == null) return;

            //�̹� �浹�ߴ� �����̶�� ����
            if (_hitUnits.Contains(hitUnit)) {
                return;
            }

            _onHit.Invoke(hitUnit);
            _hitUnits.Add(hitUnit);

            //���� Ƚ���� ������ ��, ������ �Ǿ��ٸ� �ı�
            _penetrateCount--;
            if (_penetrateCount < 0) Destroy(gameObject);
        }
    }
}
