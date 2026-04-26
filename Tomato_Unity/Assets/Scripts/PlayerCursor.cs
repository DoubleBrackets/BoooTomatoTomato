using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     STS style networked cursor
/// </summary>
public class PlayerCursor : NetworkBehaviour
{
    public override void OnStartClient()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        pos.z = 0f;
        transform.position = pos;
    }

    public override void OnStopClient()
    {
        Cursor.visible = true;
    }
}