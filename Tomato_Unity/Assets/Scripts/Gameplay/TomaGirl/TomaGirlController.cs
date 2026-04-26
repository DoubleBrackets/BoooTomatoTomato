using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<TomaGirlHitbox> _hitboxes = new();

        [SerializeField]
        private ImpactSprite _impactSprite;

        public event Action<ThrowableObjectInfo> OnThrowableHit;

        private readonly SyncVar<string> _currentAnimName = new();

        private void Awake()
        {
            foreach (TomaGirlHitbox hitbox in _hitboxes)
            {
                hitbox.OnThrowableHit += HandleThrowableHit;
            }
        }

        [ContextMenu("Detect Hitboxes")]
        private void DetectHitboxes()
        {
            _hitboxes = GetComponentsInChildren<TomaGirlHitbox>().ToList();
        }

        [Server]
        private void HandleThrowableHit(ThrowableObjectInfo info, ThrowableImpact throwableImpact)
        {
            _currentAnimName.Value = info.GetAnimationName;
            RpcPlayAnim(_currentAnimName.Value);

            Vector3 impactPoint = throwableImpact.ImpactPoint;

            RpcSpawnImpactSprite(impactPoint, info.ImpactSprite);

            OnThrowableHit?.Invoke(info);
        }

        /// <summary>
        ///     Don't bother to buffer
        /// </summary>
        /// <param name="position"></param>
        [ObserversRpc(RunLocally = true)]
        private void RpcSpawnImpactSprite(Vector3 position, string impactSprite)
        {
            ImpactSprite impactSpriteObj = Instantiate(_impactSprite, position, Quaternion.identity);
            impactSpriteObj.Initialize(impactSprite);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void RpcPlayAnim(string animName)
        {
            _animator.Play(animName);
        }
    }
}