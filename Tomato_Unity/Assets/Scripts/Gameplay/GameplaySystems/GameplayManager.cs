using FishNet.Object;
using FishNet.Object.Synchronizing;
using Gameplay.Throwables;
using Gameplay.TomaGirl;
using Script.DevTools;
using TMPro;
using UnityEngine;

namespace Gameplay.GameplaySystems
{
    /// <summary>
    ///     Giant state machine for managing gameplay states.
    /// </summary>
    public class GameplayManager : NetworkBehaviour
    {
        public enum GameplayState
        {
            BeforeInitialize,
            WaitingToBegin,
            Gameplay,
            EndScreen
        }

        [Header("Depends")]

        [SerializeField]
        private ScoringSystem _scoringSystem;

        [SerializeField]
        private TomaGirlController _tomaGirlController;

        [SerializeField]
        private WaitingToBeginManager _waitingToBeginManager;

        [SerializeField]
        private EndScreenManager _endScreenManager;

        [Header("UI")]

        [SerializeField]
        private TMP_Text _roundTimerText;

        [Header("Config")]

        [SerializeField]
        private float _roundDuration;

        private GameplayState _currentGameplayState = GameplayState.BeforeInitialize;

        private readonly SyncVar<float> _roundTimer = new();

        public override void OnStartServer()
        {
            // Enter waiting to begin
            ChangeGameplayState(GameplayState.WaitingToBegin);
        }

        public override void OnStopServer()
        {
            _tomaGirlController.OnThrowableHit -= HandleThrowableHitTomaGirl;
        }

        /// <summary>
        ///     Sync the client's gameplay state with the server.
        ///     BufferLast should sync to any new players
        /// </summary>
        /// <param name="newState"></param>
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void ChangeGameplayState(GameplayState newState)
        {
            Debug.Log($"ChangeGameplayState: {_currentGameplayState} -> {newState}");
            ExitGameplayState(_currentGameplayState);
            EnterNewGameplayState(newState);
        }

        private void EnterNewGameplayState(GameplayState newState)
        {
            _currentGameplayState = newState;
            switch (newState)
            {
                case GameplayState.BeforeInitialize:
                    // Do nothing
                    break;
                case GameplayState.WaitingToBegin:
                    _waitingToBeginManager.Enter();
                    if (IsServerInitialized)
                    {
                        _scoringSystem.ResetScore();
                        _waitingToBeginManager.OnBeginGameplay += HandleBeginGameplay;
                    }

                    break;
                case GameplayState.Gameplay:
                    _roundTimerText.gameObject.SetActive(true);
                    _scoringSystem.SetScoreUIVisible(true);

                    if (IsServerInitialized)
                    {
                        _tomaGirlController.OnThrowableHit += HandleThrowableHitTomaGirl;
                        _roundTimer.Value = _roundDuration;
                    }

                    break;
                case GameplayState.EndScreen:
                    _scoringSystem.SetScoreUIVisible(true);
                    _endScreenManager.Enter();
                    if (IsServerInitialized)
                    {
                        _endScreenManager.OnRestartGameplay += HandleRestartGameplay;
                    }

                    break;
            }
        }

        private void ExitGameplayState(GameplayState oldState)
        {
            switch (oldState)
            {
                case GameplayState.BeforeInitialize:
                    // Do nothing
                    break;
                case GameplayState.WaitingToBegin:
                    _waitingToBeginManager.Exit();
                    if (IsServerInitialized)
                    {
                        _scoringSystem.SetScoreUIVisible(false);
                        _waitingToBeginManager.OnBeginGameplay -= HandleBeginGameplay;
                    }

                    break;
                case GameplayState.Gameplay:
                    _roundTimerText.gameObject.SetActive(false);
                    _scoringSystem.SetScoreUIVisible(false);

                    if (IsServerInitialized)
                    {
                        _tomaGirlController.OnThrowableHit -= HandleThrowableHitTomaGirl;
                    }

                    break;
                case GameplayState.EndScreen:
                    _scoringSystem.SetScoreUIVisible(false);
                    _endScreenManager.Exit();
                    if (IsServerInitialized)
                    {
                        _endScreenManager.OnRestartGameplay -= HandleRestartGameplay;
                    }

                    break;
            }
        }

        [Server]
        private void HandleBeginGameplay()
        {
            ChangeGameplayState(GameplayState.Gameplay);
        }

        [Server]
        private void HandleRestartGameplay()
        {
            ChangeGameplayState(GameplayState.WaitingToBegin);
        }

        private void Update()
        {
            OnGUIHook.SetElement("State", $"{_currentGameplayState}");
            int roundTimerCeil = Mathf.CeilToInt(_roundTimer.Value);
            _roundTimerText.text = $"{roundTimerCeil}";

            if (!IsServerInitialized)
            {
                return;
            }

            if (_currentGameplayState == GameplayState.Gameplay)
            {
                _roundTimer.Value -= Time.deltaTime;
                if (_roundTimer.Value <= 0)
                {
                    ChangeGameplayState(GameplayState.EndScreen);
                }
            }
        }

        [Server]
        private void HandleThrowableHitTomaGirl(ThrowableObjectInfo info)
        {
            Debug.Log($"Received throwable hit with happiness change: {info.HappinessChange}");

            if (info.HappinessChange > 0)
            {
                _scoringSystem.IncrementHappyImpactCount();
            }
            else
            {
                _scoringSystem.IncrementSadImpactCount();
            }
        }
    }
}