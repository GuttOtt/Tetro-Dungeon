using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSPUMControl : MonoBehaviour
{
    public SPUM_Prefabs _prefabs; //SPUM 프리펩
    private PlayerState _currentState; //현재 상태

    public Dictionary<PlayerState, int> IndexPair = new();

    public void SetSPUM(SPUM_Prefabs prefabs) {
        _prefabs = prefabs;

        if (!_prefabs.allListsHaveItemsExist()) {
            _prefabs.PopulateAnimationLists();
        }

        _prefabs.OverrideControllerInit();

        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState))) {
            IndexPair[state] = 0;
        }
    }

    private void Update() {
    }

    public void ChangeState(PlayerState state) {
        _currentState = state;
        PlayStateAnimation(state);
    }

    public void PlayMoveAnimation() {
        PlayStateAnimation(PlayerState.MOVE);
    }

    public void PlayDeathAnimation() {
        PlayStateAnimation(PlayerState.DEATH);
    }

    public void PlayAttackAnimation(BaseUnit target) {
        //DoTween 움직임
        if (target == null) {
            return;
        }

        Vector3 targetPos = (target).transform.position;
        Vector3 moveVector = transform.position + (targetPos - transform.position).normalized * 0.3f;

        //DOTween과 Spum을 이용해 애니메이션 출력
        transform.DOMove(moveVector, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);

        PlayStateAnimation(PlayerState.ATTACK);

    }


    public bool IsCurrentAnimationTimePassed(float time = 1f) {
        if (_prefabs._anim == null) {
            return true;
        }

        return _prefabs._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= time;
    }
    
    private void PlayStateAnimation(PlayerState state) {
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }
}
