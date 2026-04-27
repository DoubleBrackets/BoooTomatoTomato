using System.Threading;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Object;
using Gameplay.TomaGirl;
using UnityEngine;

namespace Gameplay.Throwables
{
    public class BasicThrowable : NetworkBehaviour, IThrowableObject
    {
        [SerializeField]
        private ThrowableObjectInfo _info;

        [SerializeField]
        private Vector3 _startVelocity = new(0, 7, 4.5f);

        [SerializeField]
        private ImpactSprite _impactSpritePrefab;

        [SerializeField]
        private float _aimDepth;

        private bool _alreadyImpacted;

        private Vector3 _direction;

        public ThrowableObjectInfo GetInfo()
        {
            return _info;
        }

        public bool AlreadyImpacted => _alreadyImpacted;

        [Server]
        public void Impact(ThrowableImpact impact)
        {
            _alreadyImpacted = true;
            RpcSpawnImpactSprite(impact.ImpactPoint);
            Despawn(gameObject);
        }

        public override void OnSpawnServer(NetworkConnection connection)
        {
            var rb = GetComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.linearVelocity = _startVelocity.magnitude * _direction.normalized;

            StartDespawnTimer(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid StartDespawnTimer(CancellationToken token)
        {
            await UniTask.WaitForSeconds(3f);

            if (token.IsCancellationRequested)
            {
                return;
            }

            Despawn(gameObject);
        }

        /// <summary>
        ///     Don't bother to buffer
        /// </summary>
        /// <param name="position"></param>
        [ObserversRpc(RunLocally = true)]
        private void RpcSpawnImpactSprite(Vector3 position)
        {
            ImpactSprite impactSpriteObj = Instantiate(_impactSpritePrefab, position, Quaternion.identity);
            impactSpriteObj.Initialize(_info.ImpactSprite);
        }

        /*
        [SerializeField]
        private Vector3 _startVelocity = new(0, 7, 4.5f));

        private Transform _transform;
        private Vector3 _velocity;

        public override void OnStartNetwork()
        {
            TimeManager.OnTick += TimeManager_OnTickHandler;
            TimeManager.OnPostTick += TimeManager_OnPostTickHandler;
        }

        private void TimeManager_OnPostTickHandler()
        {
            CreateReconcile();
        }

        private void TimeManager_OnTickHandler()
        {
            if (IsOwner)
            {
                ReplicationData data = new(_jumped);
                Replicate(data);
                return;
            }

            Replicate(default(ReplicationData));
        }

        [Replicate]
        private void Replicate(ReplicationData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {

        }

        public override void CreateReconcile()
        {
            ReconciliationData data = new();
        }

        public struct ReplicationData : IReplicateData
        {
            private uint _tick;
            public readonly bool Jump;

            public ReplicationData(bool jump) : this()
            {
                Jump = jump;
            }

            public void Dispose() { }

            public uint GetTick()
            {
                return _tick;
            }

            public void SetTick(uint value)
            {
                _tick = value;
            }
        }

        public struct ReconciliationData : IReconcileData
        {
            private uint _tick;

            public void Dispose() { }

            public uint GetTick()
            {
                return _tick;
            }

            public void SetTick(uint value)
            {
                _tick = value;
            }
        }

        public override void OnStopNetwork()
        {
            TimeManager.OnTick -= TimeManager_OnTickHandler;
            TimeManager.OnPostTick -= TimeManager_OnPostTickHandler;
        }

        public override void OnStartClient()
        {
            if (!IsOwner) return;

            GetComponent<PlayerInput>().currentActionMap.FindAction("Attack").performed += OnAttack;
        }

        public void OnAttack(CallbackContext ctx)
        {
            if (!IsOwner) return;
            if (!ctx.performed) return;
            _jumped = true;
        }

        [ServerRpc]
        private void SpawnThrowable()
        {
            GameObject obj = Instantiate(_throwable, transform.position, Quaternion.identity);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.linearVelocity = _startVelocity;
            Spawn(obj);
            //ApplyVelocity(obj);
        }

        [ObserversRpc]
        private void ApplyVelocity(GameObject obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.linearVelocity = _startVelocity;
        }
        */
        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }
    }
}