using System;
using System.Collections.Generic;
using FishNet.Object;
using Gameplay.BullyController;
using Gameplay.GameplaySystems;
using Gameplay.Throwables;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

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
    private List<SelectWeaponButton> _weaponButtons;

    private BasicThrowable _selectedThrowable;

    public override void OnStartClient()
    {
        if (IsOwner)
        {
            foreach (SelectWeaponButton button in _weaponButtons)
            {
                button.OnWeaponSelected += HandleWeaponSelected;
            }

            Debug.Log("LISTENING");
            GameplayManager.Instance.OnGameplayStateChanged.AddListener(HandleGameplayStateChanged);
        }
    }

    private void HandleGameplayStateChanged(GameplayManager.GameplayState state)
    {
        Debug.Log("ASDF");
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
        SpawnThrowable();
    }

    [ServerRpc]
    private void SpawnThrowable()
    {
        if (GameplayManager.Instance.CurrentGameplayState != GameplayManager.GameplayState.Gameplay)
        {
            return;
        }

        if (_selectedThrowable == null)
        {
            _selectedThrowable = _throwableInfos[0].Throwable;
            return;
        }

        Debug.Log("spawn tomato");
        BasicThrowable obj = Instantiate(_selectedThrowable, _startPos, Quaternion.identity);
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