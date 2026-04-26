using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Gameplay.Throwables;
using UnityEngine;
using UnityEngine.InputSystem;

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
        private Rigidbody _rb;

        [SerializeField]
        private float _acceleration;

        [SerializeField]
        private InputAction _moveAction;

        [SerializeField]
        private Transform _initialSpawnPos;

        public event Action<ThrowableObjectInfo> OnThrowableHit;

        private readonly SyncVar<string> _currentAnimName = new();

        private Vector2 _moveInput;

        private bool _canMove;

        private void Awake()
        {
            foreach (TomaGirlHitbox hitbox in _hitboxes)
            {
                hitbox.OnThrowableHit += HandleThrowableHit;
            }
        }

        public override void OnStartServer()
        {
            _moveAction.performed += OnMoveInput;
            _moveAction.canceled += OnMoveInput;
        }

        public override void OnStopServer()
        {
            _moveAction.performed -= OnMoveInput;
            _moveAction.canceled -= OnMoveInput;
        }

        private void OnMoveInput(InputAction.CallbackContext obj)
        {
            _moveInput = obj.ReadValue<Vector2>();
        }

        [Server]
        public void EnterWaitToBegin()
        {
            _canMove = false;
            _rb.transform.position = _initialSpawnPos.position;
        }

        [Server]
        public void EnterGameplay()
        {
            _canMove = true;
        }

        [Server]
        public void ExitGameplay()
        {
            _canMove = false;
        }

        private void Update()
        {
            if (!IsServerInitialized || !_canMove)
            {
            }

            // No moving rip
            /*Vector3 curVel = _rb.linearVelocity;
            curVel.x += _moveInput.x * _acceleration * Time.deltaTime;
            _rb.linearVelocity = curVel;*/
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
            OnThrowableHit?.Invoke(info);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void RpcPlayAnim(string animName)
        {
            _animator.Play(animName);
        }
    }
}