using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class UnitAnimation : MonoBehaviour {
    private UnitAnimatorControl animatorControl;
    private UnitSPUMControl unitSPUMControl;
    private bool isUsingSPUM;

    public bool IsUsingSPUM => isUsingSPUM;

    public void InitBySPUM(SPUM_Prefabs prefabs) {
        unitSPUMControl = gameObject.AddComponent<UnitSPUMControl>();
        unitSPUMControl.SetSPUM(prefabs);
        isUsingSPUM = true;
    }

    public void InitByAnimator(Animator animator) {
        animatorControl = gameObject.AddComponent<UnitAnimatorControl>();
        animatorControl.SetAnimator(animator);
        isUsingSPUM = false;
    }

    public void ChangeState(PlayerState state) {
        switch (state) {
            case PlayerState.MOVE:
                PlayMoveAnimation();
                break;
            case PlayerState.IDLE:
                PlayStateAnimation(PlayerState.IDLE);
                break;
        }
    }

    public void PlayStateAnimation(PlayerState state) {
        if (isUsingSPUM) {
            unitSPUMControl.ChangeState(state);
        } else {
            animatorControl.ChangeState(state);
        }
    }

    public void PlayAttackAnimation(BaseUnit mainTarget) {
        if (isUsingSPUM) {
            unitSPUMControl.PlayAttackAnimation(mainTarget);
        } else {
            animatorControl.PlayAttackAnimation();
        }
    }

    public void PlayMoveAnimation() {
        if (isUsingSPUM) {
            unitSPUMControl.PlayMoveAnimation();
        } else {
            animatorControl.PlayMoveAnimation();
        }
    }

    public void PlayDeathAnimation() {
        if (isUsingSPUM) {
            unitSPUMControl.PlayDeathAnimation();
        } else {
            animatorControl.PlayDeathAnimation();
        }
    }

    public bool IsCurrentAnimationTimePassed(float time = 1f) {
        if (isUsingSPUM) {
            return unitSPUMControl.IsCurrentAnimationTimePassed(time);
        } else {
            return animatorControl.IsCurrentAnimationTimePassed(time);
        }
    }

    public async UniTask ChangeColor(Color color, float duration) {
        if (isUsingSPUM) {
            unitSPUMControl.ChangeColor(color);
            await UniTask.Delay((int)(duration * 1000));
            unitSPUMControl.ResetColor();
        } else {
            animatorControl.ChangeColor(color);
            await UniTask.Delay((int)(duration * 1000));
            animatorControl.ChangeColor(Color.white);
        }
    }
}
