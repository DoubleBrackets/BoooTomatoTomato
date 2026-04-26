using FishNet.Object;
using Gameplay.Throwables;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class BullyController : NetworkBehaviour
{
    [SerializeField]
    GameObject throwable;

    public void OnAttack(CallbackContext ctx)
    {
        Debug.Log("owner check");
        if (!IsOwner) return;
        Debug.Log("attack");
        if (!ctx.performed) return;
        SpawnThrowable();
        Debug.Log("attacked");
    }

    [ServerRpc]
    private void SpawnThrowable()
    {
        Debug.Log("spawn tomato");
        GameObject obj = Instantiate(throwable, transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody>().linearVelocity = (transform.forward * 5) + (transform.up * 10);
        Spawn(obj);
    }
}
