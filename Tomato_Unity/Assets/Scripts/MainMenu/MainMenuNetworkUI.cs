using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuNetworkUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _codeInputField;

        [SerializeField]
        private Button _joinButton;

        [SerializeField]
        private Button _hostButton;

        private UnityRelayManager _relayManager;

        private void Awake()
        {
            _relayManager = UnityRelayManager.Instance;

            _joinButton.onClick.AddListener(HandleJoinButtonClick);
            _hostButton.onClick.AddListener(HandleHostButtonClick);
        }

        private void OnDestroy()
        {
            _joinButton.onClick.RemoveListener(HandleJoinButtonClick);
            _hostButton.onClick.RemoveListener(HandleHostButtonClick);
        }

        private void HandleJoinButtonClick()
        {
            _relayManager.JoinGame(_codeInputField.text, destroyCancellationToken).Forget();
        }

        private void HandleHostButtonClick()
        {
            _relayManager.HostGame().Forget();
        }
    }
}