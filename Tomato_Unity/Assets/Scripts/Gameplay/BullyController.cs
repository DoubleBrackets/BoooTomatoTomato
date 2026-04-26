using FishNet.Object;
using Gameplay.Throwables;
using UnityEngine;
using UnityEngine.InputSystem;

public class BullyController : NetworkBehaviour
{
    [SerializeField]
    InputActionMap _input;

    [SerializeField]
    GameObject throwable;

    /*
    private void Awake()
    {
        _input = GetComponent<PlayerInput>().currentActionMap;
    }

    private void Update()
    {
        if (_input["Attack"].triggered)
        {
            Debug.Log("Spawn");
            SpawnThrowable();
        }
    }

    [ServerRpc]
    private void SpawnThrowable()
    {
        GameObject obj = Instantiate(throwable, transform.position, Quaternion.identity);
        obj.GetComponent<Rigidbody>().linearVelocity = (transform.forward * 5) + (transform.up * 10);
        Spawn(obj);
    }
    */
}
