using FishNet;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _joinCode;

        [SerializeField]
        private Button _disconnectButton;

        private void Awake()
        {
            _joinCode.text = UnityRelayManager.Instance.JoinCode;

            _disconnectButton.onClick.AddListener(HandleDisconnectButtonClick);
        }

        private void OnDestroy()
        {
            _disconnectButton.onClick.RemoveListener(HandleDisconnectButtonClick);
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