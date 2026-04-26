using FishNet.Object;
using Gameplay.GameplaySystems;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class BullyController : NetworkBehaviour
{
    [SerializeField]
    private GameObject _throwable;

    [SerializeField]
    private Vector3 _startPos = Vector3.forward * -5;

    public void OnAttack(CallbackContext ctx)
    {
        if (!IsOwner)
        {
            return;
        }

        if (!ctx.performed)
        {
            return;
        }

        Debug.Log("throw");
        SpawnThrowable();
    }

    [ServerRpc]
    private void SpawnThrowable()
    {
        if (GameplayManager.Instance.CurrentGameplayState != GameplayManager.GameplayState.Gameplay)
        {
            return;
        }

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