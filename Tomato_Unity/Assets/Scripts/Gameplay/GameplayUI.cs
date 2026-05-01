using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using Gameplay.GameplaySystems;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameplayUI : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text _joinCode;

        [SerializeField]
        private GameObject _gameplayUIContainer;

        [SerializeField]
        private GameplayManager _manager;

        [SerializeField]
        private Button _disconnectButton;

        [SerializeField]
        private Button _startButton;

        private NetworkManager _networkManager;

        private void Awake()
        {
            _joinCode.text = UnityRelayManager.Instance.JoinCode;

            _disconnectButton.onClick.AddListener(HandleDisconnectButtonClick);

            _networkManager = InstanceFinder.NetworkManager;

            if (!InstanceFinder.IsServerStarted)
            {
                Destroy(_startButton.transform.parent.gameObject);
                return;
            }

            // _startButton.interactable = false;
            _networkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
        }

        private void OnDestroy()
        {
            _disconnectButton.onClick.RemoveListener(HandleDisconnectButtonClick);
        }

        private void ServerManager_OnRemoteConnectionState(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
        {
            Debug.Log(_networkManager.ServerManager.Clients.Count);
            if (arg2.ConnectionState != RemoteConnectionState.Stopped)
            {
                // _startButton.interactable = true;
                return;
            }

            if (_networkManager.ServerManager.Clients.Count - 1 > 1)
            {
                return;
            }

            // _startButton.interactable = false;
        }

        private void HandleDisconnectButtonClick()
        {
            if (InstanceFinder.NetworkManager.IsHostStarted)
            {
                InstanceFinder.ServerManager.StopConnection(true);
            }

            if (InstanceFinder.NetworkManager.IsClientStarted)
            {
                InstanceFinder.ClientManager.StopConnection();
            }
        }
    }

    /*
     *         foreach (var client in InstanceFinder.ServerManager.Clients)
        {
            GameObject obj;
            /*
             * here lies playable tomagirl 2026-2026
            if (client.Value.IsHost)
            {
                obj = Instantiate(_victim);
                Spawn(obj, client.Value);
                continue;
            }


    Debug.Log(client.Value.ClientId);
            obj = Instantiate(_bully);
    Spawn(obj, client.Value);
    Debug.Log($"spawned bully with {client.Value.ClientId} as owner");
        }
    */
}