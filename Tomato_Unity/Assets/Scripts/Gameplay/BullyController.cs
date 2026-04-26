using FishNet.Object;
using Gameplay.Throwables;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class BullyController : NetworkBehaviour
{
    [SerializeField]
    GameObject _throwable;

    [SerializeField]
    Vector3 _startVelocity;

    public void OnAttack(CallbackContext ctx)
    {
        if (!IsOwner) return;
        if (!ctx.performed) return;
        SpawnThrowable();
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
}
