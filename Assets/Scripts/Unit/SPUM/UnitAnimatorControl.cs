using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimatorControl : MonoBehaviour {
    [SerializeField] private Animator animator;

    public void SetAnimator(Animator animator) {
        this.animator = animator;
    }

    public void ChangeState(PlayerState state) {
        switch (state) {
            case PlayerState.ATTACK:
                PlayAttackAnimation();
                break;
            case PlayerState.MOVE:
                PlayMoveAnimation();
                break;
            case PlayerState.IDLE:
                animator.SetBool("Move", false);
                break;
        }
    }

    public void PlayStateAnimation(PlayerState state) {
    }

    public void PlayAttackAnimation() {
        animator.SetTrigger("Attack");
    }

    public void PlayMoveAnimation() {
        animator.SetBool("Move", true);
    }

    public bool IsCurrentAnimationTimePassed(float time = 1f) {
        return true;
    }
}
