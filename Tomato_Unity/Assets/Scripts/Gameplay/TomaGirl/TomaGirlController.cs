using System;
using FishNet.Object;
using Gameplay.Throwables;
using UnityEngine;

namespace Gameplay.TomaGirl
{
    /// <summary>
    ///     Tomato target. Handled completely server side
    /// </summary>
    public class TomaGirlController : NetworkBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private TomaGirlHitbox _hitbox;

        public event Action<ThrowableObjectInfo> OnThrowableHit;

        private void Awake()
        {
            _hitbox.OnThrowableHit += HandleThrowableHit;
        }

        [Server]
        private void HandleThrowableHit(ThrowableObjectInfo info)
        {
            _animator.Play(info.GetAnimationName);
        }
    }
}