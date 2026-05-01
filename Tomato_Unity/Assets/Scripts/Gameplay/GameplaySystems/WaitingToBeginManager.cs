using System;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Gameplay.GameplaySystems
{
    /// <summary>
    ///     Handles waiting to begin gameplay stuff.
    ///     All the cutscene logic handled server side,
    ///     with individual things (e.g transforms) synced to clients.
    ///     No idea if that's a good idea but ehhhhhhhhhhhhhhhh
    ///     Maybe we should have all clients just run the cutscene stuff locally?
    /// </summary>
    public class WaitingToBeginManager : NetworkBehaviour
    {
        public event Action OnBeginGameplay;

        [Header("Depends")]

        [SerializeField]
        private Button _beginGameplayButton;

        [Header("Config")]

        [SerializeField]
        private PlayableDirector _director;

        public void Enter()
        {
            if (IsServerInitialized)
            {
                _beginGameplayButton.onClick.AddListener(HandleBeginButtonClick);
                _beginGameplayButton.transform.parent.gameObject.SetActive(true);
            }
        }

        public void Exit()
        {
            if (IsServerInitialized)
            {
                _beginGameplayButton.onClick.RemoveListener(HandleBeginButtonClick);
                _beginGameplayButton.transform.parent.gameObject.SetActive(false);
            }
        }

        private void HandleBeginButtonClick()
        {
            Debug.Log("HANDLE CLICKED");
            OnBeginGameplay?.Invoke();
        }
    }
}