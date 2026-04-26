using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Networking;
using TMPro;
using Unity.Services.Relay.Models;
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

        [SerializeField]
        private TMP_Dropdown _regionDropdown;

        private UnityRelayManager _relayManager;

        private void Awake()
        {
            _relayManager = UnityRelayManager.Instance;

            _joinButton.onClick.AddListener(HandleJoinButtonClick);
            _hostButton.onClick.AddListener(HandleHostButtonClick);
            _relayManager.OnInitialized += HandleRelayManagerInitialized;
        }

        private void OnDestroy()
        {
            _joinButton.onClick.RemoveListener(HandleJoinButtonClick);
            _hostButton.onClick.RemoveListener(HandleHostButtonClick);
            _relayManager.OnInitialized -= HandleRelayManagerInitialized;
        }

        private void HandleRelayManagerInitialized(UnityRelayManager.AuthenticationEventData authenticationEventData)
        {
            if (authenticationEventData.IsAuthenticated)
            {
                LoadRegionDropdown().Forget();
            }
        }

        private async UniTaskVoid LoadRegionDropdown()
        {
            List<Region> regions = await _relayManager.GetRegionList();
            List<TMP_Dropdown.OptionData> options = regions.Select(r => new TMP_Dropdown.OptionData
            {
                text = r.Id
            }).ToList();

            var isWebGL = false;
#if UNITY_WEBGL && !UNITY_EDITOR
            isWebGL = true;
#endif

            options.Reverse();

            if (!isWebGL)
            {
                options.Insert(0, new TMP_Dropdown.OptionData { text = "Auto" });
            }

            _regionDropdown.options = options;
        }

        private void HandleJoinButtonClick()
        {
            _relayManager.JoinGame(_codeInputField.text, destroyCancellationToken).Forget();
        }

        private void HandleHostButtonClick()
        {
            string itemText = _regionDropdown.options[_regionDropdown.value].text;
            _relayManager.HostGame(itemText == "Auto" ? null : itemText).Forget();
        }
    }
}