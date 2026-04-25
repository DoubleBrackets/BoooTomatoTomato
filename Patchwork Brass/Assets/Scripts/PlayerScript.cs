using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField]
    private InputAction _moveAction;

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private Transform _targetTransform;

    private Vector2 _moveDirection;

    private void OnEnable()
    {
        _moveAction.Enable();
        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMovePerformed;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMovePerformed;
        _moveAction.Disable();
    }

    protected override void OnOwnerDisconnected(PlayerID ownerId)
    {
        Despawn();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _moveDirection = ctx.ReadValue<Vector2>();
    }

    public void Update()
    {
        if (!isOwner)
        {
            return;
        }

        _targetTransform.position +=
            new Vector3(_moveDirection.x, _moveDirection.y, 0).normalized * _moveSpeed * Time.deltaTime;
    }
}