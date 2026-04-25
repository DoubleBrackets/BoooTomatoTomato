using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class NetworkUI : MonoBehaviour
    {
        [SerializeField]
        private UnityRelayManager _relayManager;

        [SerializeField]
        private TMP_InputField _codeInputField;

        [SerializeField]
        private Button _joinButton;

        [SerializeField]
        private Button _hostButton;

        [SerializeField]
        private TMP_Text _joinCode;

        private void Awake()
        {
            _joinButton.onClick.AddListener(HandleJoinButtonClick);
            _hostButton.onClick.AddListener(HandleHostButtonClick);

            _relayManager.OnJoinAllocationEvent += HandleJoinAllocationEvent;
            _relayManager.OnCreateAllocationEvent += HandleCreateAllocationEvent;
        }

        private void OnDestroy()
        {
            _joinButton.onClick.RemoveListener(HandleJoinButtonClick);
            _hostButton.onClick.RemoveListener(HandleHostButtonClick);

            _relayManager.OnJoinAllocationEvent -= HandleJoinAllocationEvent;
            _relayManager.OnCreateAllocationEvent -= HandleCreateAllocationEvent;
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
    }
}