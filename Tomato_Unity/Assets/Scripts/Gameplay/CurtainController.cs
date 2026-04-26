using FishNet.Object;
using Gameplay.GameplaySystems;
using System;
using UnityEngine;

public class CurtainController : MonoBehaviour
{
    private GameplayManager _manager;
    private Animator _animator;

    public void Awake()
    {
        _manager.OnGameplayStateChanged.AddListener(GamepalyManager_OnGameplayStateChangedHandler);
        _animator = GetComponent<Animator>();
    }

    private void GamepalyManager_OnGameplayStateChangedHandler(GameplayManager.GameplayState state)
    {
        if (state == GameplayManager.GameplayState.Gameplay)
        {
            Debug.Log("start");
            _animator.Play("RaiseCurtains", 0);
        }
        if (state == GameplayManager.GameplayState.EndScreen)
        {
            Debug.Log("stop");
            _animator.Play("LowerCurtains", 0);
        }
    }
}
