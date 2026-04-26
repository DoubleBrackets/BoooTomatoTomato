using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GameplaySystems
{
    /// <summary>
    ///     Handles end screen stuff.
    ///     Also follows "Server does all the logic, clients just sync the presentation"
    /// </summary>
    public class EndScreenManager : NetworkBehaviour
    {
        public event Action OnRestartGameplay;

        [Header("Depends")]

        [SerializeField]
        private Button _restartButton;

        public void Enter()
        {
            _restartButton.gameObject.SetActive(IsServerInitialized);

            if (IsServerInitialized)
            {
                _restartButton.onClick.AddListener(HandleRestartButtonClick);
            }
        }

        public void Exit()
        {
            if (IsServerInitialized)
            {
                _restartButton.onClick.RemoveListener(HandleRestartButtonClick);
            }

            _restartButton.gameObject.SetActive(false);
        }

        private void HandleRestartButtonClick()
        {
            OnRestartGameplay?.Invoke();
        }
    }
}