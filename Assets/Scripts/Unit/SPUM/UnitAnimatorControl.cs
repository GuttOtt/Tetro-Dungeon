using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UnitAnimatorControl : MonoBehaviour {
    [SerializeField] private Animator animator;
    private bool isDead = false;

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

    public void PlayDeathAnimation() {
        if (isDead) return;

        isDead = true;
        animator.SetBool("Death", true);
        Fade(1.5f).Forget();
    }

    public async UniTask Fade(float duration) {
        SpriteRenderer sr = animator.GetComponent<SpriteRenderer>();
        if (sr == null) {
            Debug.LogWarning("SpriteRenderer not found on the animator's GameObject.");
            return;
        }
        await sr.DOFade(0, duration).AsyncWaitForCompletion();
    }

    public bool IsCurrentAnimationTimePassed(float time = 1f) {
        return true;
    }
}
