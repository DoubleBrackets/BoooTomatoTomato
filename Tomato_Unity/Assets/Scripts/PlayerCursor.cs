using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     STS style networked cursor
/// </summary>
public class PlayerCursor : NetworkBehaviour
{
    private Vector3 _prevPos = Vector3.zero;
    private Queue<Vector3> _deltaBuffer = new();
    private int _deltaBufferCount = 20;

    private Vector3 _avgDelta = Vector3.zero;

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
        Vector3 delta = pos - _prevPos;

        _deltaBuffer.Enqueue(delta);
        _avgDelta += delta / _deltaBufferCount;
        if (_deltaBuffer.Count > _deltaBufferCount)
        {
            _avgDelta -= _deltaBuffer.Dequeue() / _deltaBufferCount;
        }

        float speed = _avgDelta.magnitude / Time.deltaTime;

        if (speed > 0.1f)
        {
            Quaternion currentRot = Quaternion.Euler(transform.rotation.eulerAngles);
            Quaternion desiredRot = Quaternion.Euler(0f, 0f, Mathf.Atan2(_avgDelta.y, _avgDelta.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(currentRot, desiredRot, Time.deltaTime * 100f);
        }

        _prevPos = pos;
    }

    public override void OnStopClient()
    {
        Cursor.visible = true;
    }
}