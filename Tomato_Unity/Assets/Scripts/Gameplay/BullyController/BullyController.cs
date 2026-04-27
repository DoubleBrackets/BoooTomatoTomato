using System;
using System.Collections.Generic;
using FishNet.Object;
using Gameplay.BullyController;
using Gameplay.GameplaySystems;
using Gameplay.Throwables;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Random = UnityEngine.Random;

public class BullyController : NetworkBehaviour
{
    [Serializable]
    public struct ThrowableInfo
    {
        public BasicThrowable Throwable;
        public string Name;
    }

    [SerializeField]
    private List<ThrowableInfo> _throwableInfos;

    [SerializeField]
    private GameObject _weaponSelectionCanvas;

    [SerializeField]
    private Vector3 _startPos = Vector3.forward * -5;

    [SerializeField]
    private float _aimDepth;

    [SerializeField]
    private float _vertAimMult;

    [SerializeField]
    private List<SelectWeaponButton> _weaponButtons;

    [SerializeField]
    private Vector2 _startHorizontalRange;

    private BasicThrowable _selectedThrowable;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            foreach (SelectWeaponButton button in _weaponButtons)
            {
                button.OnWeaponSelected += HandleWeaponSelected;
            }

            GameplayManager.Instance.OnGameplayStateChanged.AddListener(HandleGameplayStateChanged);
            HandleGameplayStateChanged(GameplayManager.Instance.CurrentGameplayState);
        }
    }

    public override void OnStartServer()
    {
        var startPos = new Vector3(Random.Range(_startHorizontalRange.x, _startHorizontalRange.y), _startPos.y,
            _startPos.z);
        RpcSetStartPosition(startPos);
    }

    [ObserversRpc(RunLocally = true)]
    private void RpcSetStartPosition(Vector3 startPos)
    {
        _startPos = startPos;
    }

    private void HandleGameplayStateChanged(GameplayManager.GameplayState state)
    {
        if (state != GameplayManager.GameplayState.Gameplay)
        {
            ExitGameplay();
        }
        else
        {
            EnterGameplay();
        }
    }

    private void HandleWeaponSelected(string throwableName)
    {
        Debug.Log($"Selected throwable in client: {throwableName}");
        SetThrowable(throwableName);
    }

    [ServerRpc(RunLocally = true)]
    private void SetThrowable(string throwableName)
    {
        _selectedThrowable =
            _throwableInfos.Find(t => t.Name == throwableName).Throwable;
    }

    [Client]
    public void EnterGameplay()
    {
        if (IsOwner)
        {
            _weaponSelectionCanvas.SetActive(true);
        }
    }

    [Client]
    public void ExitGameplay()
    {
        if (IsOwner)
        {
            _weaponSelectionCanvas.SetActive(false);
        }
    }

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
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 withDepth = new(mousePos.x, mousePos.y, _aimDepth);
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(withDepth);
        Vector3 direction = cursorPos - _startPos;

        direction.y *= _vertAimMult;
        direction.Normalize();
        SpawnThrowable(direction);
    }

    [ServerRpc]
    private void SpawnThrowable(Vector3 direction)
    {
        Debug.Log("SpawnThrowable Server");
        if (GameplayManager.Instance.CurrentGameplayState != GameplayManager.GameplayState.Gameplay)
        {
            return;
        }

        if (_selectedThrowable == null)
        {
            _selectedThrowable = _throwableInfos[0].Throwable;
        }

        Debug.Log("spawn tomato");
        BasicThrowable obj = Instantiate(_selectedThrowable, _startPos, Quaternion.identity);
        obj.SetDirection(direction);
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