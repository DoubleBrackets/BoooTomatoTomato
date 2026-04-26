using System;
using FishNet.Object;
using Gameplay.Throwables;
using UnityEngine;

namespace Gameplay.TomaGirl
{
    public class TomaGirlHitbox : NetworkBehaviour
    {
        public event Action<ThrowableObjectInfo> OnThrowableHit;

        public void OnCollisionEnter(Collision other)
        {
            if (!IsServerStarted)
            {
                return;
            }

            var throwable = other.gameObject.GetComponentInParent<IThrowableObject>();
            if (throwable == null)
            {
                return;
            }

            ThrowableObjectInfo info = throwable.GetInfo();
            if (info == null)
            {
                return;
            }

            OnThrowableHit?.Invoke(info);
        }
    }
}