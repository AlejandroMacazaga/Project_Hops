using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using MEC; // Uses More Effective Coroutines from the Unity Asset Store

public class AnimationSystem {
    PlayableGraph playableGraph;
    readonly AnimationMixerPlayable topLevelMixer;
    readonly AnimationMixerPlayable locomotionMixer;
    
    AnimationClipPlayable oneShotPlayable;
    
    CoroutineHandle blendInHandle;
    CoroutineHandle blendOutHandle;

    public AnimationSystem(Animator animator, AnimationClip idleClip, AnimationClip walkClip) {
        playableGraph = PlayableGraph.Create("AnimationSystem");
        
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
        
        topLevelMixer = AnimationMixerPlayable.Create(playableGraph, 2);
        playableOutput.SetSourcePlayable(topLevelMixer);
        
        locomotionMixer = AnimationMixerPlayable.Create(playableGraph, 2);
        topLevelMixer.ConnectInput(0, locomotionMixer, 0);
        playableGraph.GetRootPlayable(0).SetInputWeight(0, 1f);
        
        AnimationClipPlayable idlePlayable = AnimationClipPlayable.Create(playableGraph, idleClip);
        AnimationClipPlayable walkPlayable = AnimationClipPlayable.Create(playableGraph, walkClip);
        
        idlePlayable.GetAnimationClip().wrapMode = WrapMode.Loop;
        walkPlayable.GetAnimationClip().wrapMode = WrapMode.Loop;
        
        locomotionMixer.ConnectInput(0, idlePlayable, 0);
        locomotionMixer.ConnectInput(1, walkPlayable, 0);
        
        playableGraph.Play();
    }

    public void UpdateLocomotion(Vector3 velocity, float maxSpeed) {
        float weight = Mathf.InverseLerp(0f, maxSpeed, velocity.magnitude);
        locomotionMixer.SetInputWeight(0, 1f - weight);
        locomotionMixer.SetInputWeight(1, weight);
    }

    public void PlayOneShot(AnimationClip oneShotClip, bool loop = false) {
        if (oneShotPlayable.IsValid() && oneShotPlayable.GetAnimationClip() == oneShotClip) return;
        
        InterruptOneShot();
        oneShotPlayable = AnimationClipPlayable.Create(playableGraph, oneShotClip);
        topLevelMixer.ConnectInput(1, oneShotPlayable, 0);
        topLevelMixer.SetInputWeight(1, 1f);
        
        // Calculate blendDuration as 10% of clip length,
        // but ensure that it's not less than 0.1f or more than half the clip length
        float blendDuration = Mathf.Clamp(oneShotClip.length * 0.1f, 0.1f, oneShotClip.length * 0.5f);
        
        BlendIn(blendDuration);
        if (!loop) BlendOut(blendDuration, oneShotClip.length - blendDuration);
        else oneShotPlayable.GetAnimationClip().wrapMode = WrapMode.Loop;
    }

    void BlendIn(float duration) {
        blendInHandle = Timing.RunCoroutine(Blend(duration, blendTime => {
            float weight = Mathf.Lerp(1f, 0f, blendTime);
            topLevelMixer.SetInputWeight(0, weight);
            topLevelMixer.SetInputWeight(1, 1f - weight);
        }));
    }
    
    void BlendOut(float duration, float delay) {
        blendOutHandle = Timing.RunCoroutine(Blend(duration, blendTime => {
            float weight = Mathf.Lerp(0f, 1f, blendTime);
            topLevelMixer.SetInputWeight(0, weight);
            topLevelMixer.SetInputWeight(1, 1f - weight);
        }, delay, DisconnectOneShot));
    }

    IEnumerator<float> Blend(float duration, Action<float> blendCallback, float delay = 0f, Action finishedCallback = null) {
        if (delay > 0f) {
            yield return Timing.WaitForSeconds(delay);
        }
        
        float blendTime = 0f;
        while (blendTime < 1f) {
            blendTime += Time.deltaTime / duration;
            blendCallback(blendTime);
            yield return blendTime;
        }
        
        blendCallback(1f);
        
        finishedCallback?.Invoke();
    }

    void InterruptOneShot() {
        Timing.KillCoroutines(blendInHandle);
        Timing.KillCoroutines(blendOutHandle);
        
        topLevelMixer.SetInputWeight(0, 1f);
        topLevelMixer.SetInputWeight(1, 0f);

        if (oneShotPlayable.IsValid()) {
            DisconnectOneShot();
        }
    }

    void DisconnectOneShot() {
        topLevelMixer.DisconnectInput(1);
        playableGraph.DestroyPlayable(oneShotPlayable);
    }

    public void Destroy() {
        if (playableGraph.IsValid()) {
            playableGraph.Destroy();
        }
    }
}