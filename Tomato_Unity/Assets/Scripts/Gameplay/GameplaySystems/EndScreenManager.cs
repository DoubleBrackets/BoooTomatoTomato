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

        [SerializeField]
        private CanvasGroup _endScreenCanvasGroup;

        [SerializeField]
        private Animator _animator;

        private void Awake()
        {
            SetVisibility(false);
        }

        public void Enter()
        {
            if (IsServerInitialized)
            {
                _restartButton.onClick.AddListener(HandleRestartButtonClick);
            }

            _restartButton.gameObject.SetActive(IsServerInitialized);
        }

        public void Exit()
        {
            if (IsServerInitialized)
            {
                _restartButton.onClick.RemoveListener(HandleRestartButtonClick);
            }

            SetVisibility(false);
        }

        private void HandleRestartButtonClick()
        {
            OnRestartGameplay?.Invoke();
        }

        private void SetVisibility(bool visible)
        {
            _endScreenCanvasGroup.alpha = visible ? 1f : 0f;
            _endScreenCanvasGroup.blocksRaycasts = visible;
        }

        [Server]
        public void ShowEndScreen(bool win)
        {
            RpcShowEndScreen(win);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void RpcShowEndScreen(bool win)
        {
            SetVisibility(true);
            _animator.Play(win ? "Win" : "Lose");
        }
    }
}