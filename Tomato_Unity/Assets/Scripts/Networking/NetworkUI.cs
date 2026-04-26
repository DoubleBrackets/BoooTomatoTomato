using FishNet;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _codeInputField;

        [SerializeField]
        private Button _joinButton;

        [SerializeField]
        private Button _hostButton;

        [SerializeField]
        private Button _disconnectButton;

        [SerializeField]
        private TMP_Text _joinCode;

        private UnityRelayManager _relayManager;

        private void Awake()
        {
            _relayManager = UnityRelayManager.Instance;

            _joinButton.onClick.AddListener(HandleJoinButtonClick);
            _hostButton.onClick.AddListener(HandleHostButtonClick);
            _disconnectButton.onClick.AddListener(HandleDisconnectButtonClick);

            _relayManager.OnJoinAllocationEvent += HandleJoinAllocationEvent;
            _relayManager.OnCreateAllocationEvent += HandleCreateAllocationEvent;

            _disconnectButton.interactable = false;

            InstanceFinder.ServerManager.OnServerConnectionState += HandleServerConnectionState;
            InstanceFinder.ClientManager.OnClientConnectionState += HandleClientConnectionState;
        }

        private void OnDestroy()
        {
            _joinButton.onClick.RemoveListener(HandleJoinButtonClick);
            _hostButton.onClick.RemoveListener(HandleHostButtonClick);
            _disconnectButton.onClick.RemoveListener(HandleDisconnectButtonClick);

            _relayManager.OnJoinAllocationEvent -= HandleJoinAllocationEvent;
            _relayManager.OnCreateAllocationEvent -= HandleCreateAllocationEvent;
            InstanceFinder.ServerManager.OnServerConnectionState -= HandleServerConnectionState;
            InstanceFinder.ClientManager.OnClientConnectionState -= HandleClientConnectionState;
        }

        private void HandleServerConnectionState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                _disconnectButton.interactable = true;
            }
            else if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                _disconnectButton.interactable = false;
            }
        }

        private void HandleClientConnectionState(ClientConnectionStateArgs args)
        {
            // Hosts use server callbacks to control disconnect button
            if (!InstanceFinder.NetworkManager.IsHostStarted)
            {
                if (args.ConnectionState == LocalConnectionState.Started)
                {
                    _disconnectButton.interactable = true;
                }
                else if (args.ConnectionState == LocalConnectionState.Stopped)
                {
                    _disconnectButton.interactable = false;
                }
            }
        }

        private void HandleJoinAllocationEvent(UnityRelayManager.JoinAllocationEventData data)
        {
            Debug.Log($"HandleJoinAllocationEvent Success: {data.DidSucceed} FailureReason: {data.FailureReason}");
            _joinCode.text = data.JoinCode;
        }

        private void HandleCreateAllocationEvent(UnityRelayManager.CreateAllocationEventData data)
        {
            Debug.Log(
                $"HandleCreateAllocationEvent Success: {data.DidSucceed} FailureReason: {data.FailureReason} JoinCode: {data.JoinCode}");
            _joinCode.text = data.JoinCode;
        }

        private void HandleJoinButtonClick()
        {
            _relayManager.JoinGame(_codeInputField.text, destroyCancellationToken).Forget();
        }

        private void HandleHostButtonClick()
        {
            _relayManager.HostGame().Forget();
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

            _joinCode.text = "";
        }
    }
}