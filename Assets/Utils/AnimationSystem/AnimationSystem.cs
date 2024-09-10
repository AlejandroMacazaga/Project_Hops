using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using MEC;
using UnityEngine.Serialization; // Uses More Effective Coroutines from the Unity Asset Store

namespace Utils.AnimationSystem
{
    [Serializable]
    public struct AnimatorConfig
    {
        public Animator animator;
        public AnimationClip idleAnimation;
        public AnimationClip movingAnimation;
    }

    public class AnimationSystem
    {
        PlayableGraph _playableGraph;
        readonly AnimationMixerPlayable _topLevelMixer;
        readonly AnimationMixerPlayable _locomotionMixer;

        AnimationClipPlayable _oneShotPlayable;

        CoroutineHandle _blendInHandle;
        CoroutineHandle _blendOutHandle;

        public AnimationSystem(AnimatorConfig animatorConfig)
        {
            _playableGraph = PlayableGraph.Create("AnimationSystem");

            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", animatorConfig.animator);

            _topLevelMixer = AnimationMixerPlayable.Create(_playableGraph, 2);
            playableOutput.SetSourcePlayable(_topLevelMixer);

            _locomotionMixer = AnimationMixerPlayable.Create(_playableGraph, 2);
            _topLevelMixer.ConnectInput(0, _locomotionMixer, 0);
            _playableGraph.GetRootPlayable(0).SetInputWeight(0, 1f);

            var idlePlayable = AnimationClipPlayable.Create(_playableGraph, animatorConfig.idleAnimation);
            var walkPlayable = AnimationClipPlayable.Create(_playableGraph, animatorConfig.movingAnimation);

            idlePlayable.GetAnimationClip().wrapMode = WrapMode.Loop;
            walkPlayable.GetAnimationClip().wrapMode = WrapMode.Loop;

            _locomotionMixer.ConnectInput(0, idlePlayable, 0);
            _locomotionMixer.ConnectInput(1, walkPlayable, 0);

            _playableGraph.Play();
        }

        public void UpdateLocomotion(Vector3 velocity, float maxSpeed)
        {
            var weight = Mathf.InverseLerp(0f, maxSpeed, velocity.magnitude);
            _locomotionMixer.SetInputWeight(0, 1f - weight);
            _locomotionMixer.SetInputWeight(1, weight);
        }

        public void PlayOneShot(AnimationClip oneShotClip, bool loop = false)
        {
            if (_oneShotPlayable.IsValid() && _oneShotPlayable.GetAnimationClip() == oneShotClip) return;
            InterruptOneShot();
            _oneShotPlayable = AnimationClipPlayable.Create(_playableGraph, oneShotClip);
            _topLevelMixer.ConnectInput(1, _oneShotPlayable, 0);
            _topLevelMixer.SetInputWeight(1, 1f);

            // Calculate blendDuration as 10% of clip length,
            // but ensure that it's not less than 0.1f or more than half the clip length
            var blendDuration = Mathf.Clamp(oneShotClip.length * 0.1f, 0.1f, oneShotClip.length * 0.5f);

            BlendIn(blendDuration);
            if (!loop) BlendOut(blendDuration, oneShotClip.length - blendDuration);
            else _oneShotPlayable.GetAnimationClip().wrapMode = WrapMode.Loop;
        }

        private void BlendIn(float duration)
        {
            _blendInHandle = Timing.RunCoroutine(Blend(duration, blendTime => {
                var weight = Mathf.Lerp(1f, 0f, blendTime);
                _topLevelMixer.SetInputWeight(0, weight);
                _topLevelMixer.SetInputWeight(1, 1f - weight);
            }));
        }

        private void BlendOut(float duration, float delay)
        {
            _blendOutHandle = Timing.RunCoroutine(Blend(duration, blendTime => {
                var weight = Mathf.Lerp(0f, 1f, blendTime);
                _topLevelMixer.SetInputWeight(0, weight);
                _topLevelMixer.SetInputWeight(1, 1f - weight);
            }, delay, DisconnectOneShot));
        }

        private static IEnumerator<float> Blend(float duration, Action<float> blendCallback, float delay = 0f, Action finishedCallback = null)
        {
            if (delay > 0f)
            {
                yield return Timing.WaitForSeconds(delay);
            }

            var blendTime = 0f;
            while (blendTime < 1f)
            {
                blendTime += Time.deltaTime / duration;
                blendCallback(blendTime);
                yield return blendTime;
            }

            blendCallback(1f);

            finishedCallback?.Invoke();
        }

        private void InterruptOneShot()
        {
            Timing.KillCoroutines(_blendInHandle);
            Timing.KillCoroutines(_blendOutHandle);

            _topLevelMixer.SetInputWeight(0, 1f);
            _topLevelMixer.SetInputWeight(1, 0f);

            if (_oneShotPlayable.IsValid())
            {
                DisconnectOneShot();
            }
        }

        private void DisconnectOneShot()
        {
            _topLevelMixer.DisconnectInput(1);
            _playableGraph.DestroyPlayable(_oneShotPlayable);
        }

        public void Destroy()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
        }
    }
}
