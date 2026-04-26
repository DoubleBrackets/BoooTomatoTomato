using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using Networking;
using System;
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
        private Button _disconnectButton;
        [SerializeField]
        private Button _startButton;

        private NetworkManager _networkManager;

        private void Awake()
        {
            _joinCode.text = UnityRelayManager.Instance.JoinCode;

            _disconnectButton.onClick.AddListener(HandleDisconnectButtonClick);
            _startButton.onClick.AddListener(HandleStartButtonClick);

            _networkManager = InstanceFinder.NetworkManager;

            if (!InstanceFinder.IsServerStarted)
            {
                Destroy(_startButton.gameObject);
                return;
            }
            _startButton.interactable = false;
            _networkManager.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
        }

        private void OnDestroy()
        {
            _disconnectButton.onClick.RemoveListener(HandleDisconnectButtonClick);
        }


        [ObserversRpc]
        private void HandleStartButtonClick()
        {
            _gameplayUIContainer.SetActive(false);

            if (!IsHostStarted) return;
            GameplayManager.Instance.OnGameStarted.Invoke();
        }

        private void ServerManager_OnRemoteConnectionState(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
        {
            Debug.Log(_networkManager.ServerManager.Clients.Count);
            if (arg2.ConnectionState != RemoteConnectionState.Stopped)
            {
                _startButton.interactable = true;
                return;
            }

            if (_networkManager.ServerManager.Clients.Count - 1 > 1) return;

            _startButton.interactable = false;

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
}