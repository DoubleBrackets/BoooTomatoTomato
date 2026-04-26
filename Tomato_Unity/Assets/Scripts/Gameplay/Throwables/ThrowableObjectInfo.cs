using System;
using UnityEngine;

namespace Gameplay.Throwables
{
    /// <summary>
    ///     Data for throwable objects
    /// </summary>
    [Serializable]
    public class ThrowableObjectInfo
    {
        [Tooltip("Name of the animation to play on tomagirl. Only used if an animation clip is not provided.")]
        public string AnimationToPlay;

        [Tooltip("Animation clip to play on tomagirl")]
        public AnimationClip AnimationClipToPlay;

        public string GetAnimationName => AnimationClipToPlay != null ? AnimationClipToPlay.name : AnimationToPlay;

        [Tooltip("How much happiness gained/lost. Happiness goes from 0 to 1")]
        public float HappinessChange;
    }
}