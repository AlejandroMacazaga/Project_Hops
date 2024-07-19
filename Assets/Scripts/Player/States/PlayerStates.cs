using Assets.Scripts.Entities;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Player
{
    public abstract class BasePlayerState : BaseEntityState
    {
        private AnimationClip AnimationClip;
        protected BasePlayerState(PlayerController controller, AnimationClip animationClip) : base(controller)
        {
            AnimationClip = animationClip;
        }

    }

}
