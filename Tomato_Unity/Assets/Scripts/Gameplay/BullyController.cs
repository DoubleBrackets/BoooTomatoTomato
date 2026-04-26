using FishNet.Connection;
using FishNet.Example.CustomSyncObject;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using Gameplay.GameplaySystems;
using Gameplay.Throwables;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class BullyController : NetworkBehaviour
{
    [SerializeField]
    private GameObject _throwable;

    [SerializeField]
    private Vector3 _startPos = Vector3.forward * -5;

    [SerializeField]
    private GameplayManager _manager;

    public override void OnStartClient()
    {
        if (IsServerInitialized)
        {
            _manager = FindAnyObjectByType<GameplayManager>();
        }
    }

    public void OnAttack(CallbackContext ctx)
    {
        if (!IsOwner) return;
        if (!ctx.performed) return;
        Debug.Log("throw");
        SpawnThrowable();
    }

    [ServerRpc]
    private void SpawnThrowable()
    {
        if (_manager.CurrentGameplayState != GameplayManager.GameplayState.Gameplay) return;

        Debug.Log("spawn tomato");
        GameObject obj = Instantiate(_throwable, _startPos, Quaternion.identity);
        Spawn(obj);
    }

    /*
    [ObserversRpc]
    private void ApplyVelocity(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = _startVelocity;
    }
    */
}
