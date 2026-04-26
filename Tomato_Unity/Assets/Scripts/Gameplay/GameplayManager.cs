using FishNet;
using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance;
    public UnityEvent OnGameStarted;

    [SerializeField]
    private GameObject _victim;
    [SerializeField]
    private GameObject _bully;

    private void Awake()
    {
        Instance = this;
        OnGameStarted.AddListener(GameplayManager_HandleGameStarted);
    }

    private void GameplayManager_HandleGameStarted()
    {
        foreach (var client in InstanceFinder.ServerManager.Clients)
        {
            GameObject obj;
            if (client.Value.IsHost)
            {
                obj = Instantiate(_victim);
                Spawn(obj, client.Value);
                continue;
            }

            obj = Instantiate(_bully);
            Spawn(obj, client.Value);
        }
    }
}
