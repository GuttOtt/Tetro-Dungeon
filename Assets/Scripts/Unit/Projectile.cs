using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField]
    private float hitDistance;

    public event Action OnHitTarget;
    public BaseUnit Target;
    public float Speed = 7;

    public void Init(BaseUnit target, Action onHitTarget, float speed = 7) {
        Target = target;
        Speed = speed;
        OnHitTarget = onHitTarget;
    }


    private void Update() {
        if (Target == null || Target.gameObject == null)
            Destroy(gameObject);

        Fly();

        if (IsHit()) {
            OnHitTarget.Invoke();
            Destroy(gameObject);
        }
    }

    private void Fly() {
        if (Target == null || Target.transform == null) return;

        Transform targetTransform = Target.transform;

        Vector2 direction = targetTransform.position - transform.position;
        Vector3 moveVector = direction.normalized * Speed * Time.deltaTime;

        transform.position += moveVector;
    }

    private bool IsHit() {
        if (Target == null || Target.transform == null) return false;

        Vector2 targetPos = Target.transform.position;
        Vector2 pos = transform.position;
        float distance = (targetPos - pos).magnitude;

        if (distance <= hitDistance) return true;
        else return false;
    }
}
