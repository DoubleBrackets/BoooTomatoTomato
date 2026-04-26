using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     STS style networked cursor
/// </summary>
public class PlayerCursor : NetworkBehaviour
{
    [SerializeField]
    private float _cameraDist;

    public override void OnStartClient()
    {
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        var pos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, _cameraDist);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        transform.position = worldPos;
    }

    private void GameplayManager_HandleGameStarted()
    {
        Despawn();
    }

    public override void OnStopClient()
    {
    }
}