using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
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

        private readonly SyncVar<string> _currentAnimName = new();

        private void Awake()
        {
            _hitbox.OnThrowableHit += HandleThrowableHit;
        }

        [Server]
        private void HandleThrowableHit(ThrowableObjectInfo info)
        {
            _currentAnimName.Value = info.GetAnimationName;
            RpcPlayAnim(_currentAnimName.Value);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void RpcPlayAnim(string animName)
        {
            _animator.Play(animName);
        }
    }
}