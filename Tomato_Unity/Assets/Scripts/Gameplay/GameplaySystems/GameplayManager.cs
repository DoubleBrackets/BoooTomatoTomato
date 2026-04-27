using FishNet.Object;
using FishNet.Object.Synchronizing;
using Gameplay.Throwables;
using Gameplay.TomaGirl;
using Script.DevTools;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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

        public static GameplayManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GameplayManager>(FindObjectsInactive.Include);
                }

                return _instance;
            }
        }

        private static GameplayManager _instance;

        [Header("Depends")]

        [SerializeField]
        private ScoringSystem _scoringSystem;

        [SerializeField]
        private TomaGirlController _tomaGirlController;

        [SerializeField]
        private GameObject _bully;

        [SerializeField]
        private WaitingToBeginManager _waitingToBeginManager;

        [SerializeField]
        private EndScreenManager _endScreenManager;

        [SerializeField]
        private JokeTellManager _jokeTellManager;

        [Header("UI")]

        [SerializeField]
        private TMP_Text _roundTimerText;

        [Header("Config")]

        [SerializeField]
        private float _roundDuration;

        private GameplayState _currentGameplayState = GameplayState.BeforeInitialize;
        public GameplayState CurrentGameplayState => _currentGameplayState;

        private readonly SyncVar<float> _roundTimer = new();

        public UnityEvent<GameplayState> OnGameplayStateChanged;

        public override void OnStartNetwork()
        {
            Debug.Log("Set GameplayManager Instance");
            _instance = this;
        }

        public override void OnStopNetwork()
        {
            _instance = null;
        }

        public override void OnStartServer()
        {
            // Enter waiting to begin
            ChangeGameplayState(GameplayState.WaitingToBegin);
        }

        public override void OnStopServer()
        {
            ExitGameplayState(_currentGameplayState);
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
            Debug.Log("INVOKED");
            OnGameplayStateChanged?.Invoke(newState);
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
                    _roundTimerText.gameObject.SetActive(false);
                    if (IsServerInitialized)
                    {
                        _tomaGirlController.EnterWaitToBegin();
                        _scoringSystem.ResetScore();
                        _waitingToBeginManager.OnBeginGameplay += HandleBeginGameplay;
                    }

                    break;
                case GameplayState.Gameplay:
                    _roundTimerText.gameObject.SetActive(true);
                    _scoringSystem.SetScoreUIVisible(true);

                    if (IsServerInitialized)
                    {
                        _tomaGirlController.EnterGameplay();
                        _tomaGirlController.OnThrowableHit += HandleThrowableHitTomaGirl;
                        _roundTimer.Value = _roundDuration;
                        _jokeTellManager.PlayJoke();
                    }

                    break;
                case GameplayState.EndScreen:
                    _endScreenManager.Enter();
                    if (IsServerInitialized)
                    {
                        _endScreenManager.OnRestartGameplay += HandleRestartGameplay;
                        _endScreenManager.ShowEndScreen(_scoringSystem.HappyImpactCount >=
                                                        _scoringSystem.SadImpactCount);
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
                        _tomaGirlController.ExitGameplay();
                        _tomaGirlController.OnThrowableHit -= HandleThrowableHitTomaGirl;
                    }

                    break;
                case GameplayState.EndScreen:
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