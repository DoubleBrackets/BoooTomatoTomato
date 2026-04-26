using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace Gameplay.GameplaySystems
{
    /// <summary>
    ///     Happiness system, logic handled server side and synced to clients.
    /// </summary>
    public class ScoringSystem : NetworkBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private Slider _balanceBar;

        [SerializeField]
        private GameObject _scoreUI;

        [SerializeField]
        private TMP_Text _happyImpactCountText;

        [SerializeField]
        private TMP_Text _sadImpactCountText;

        /// <summary>
        ///     Current happiness value, from 0 to 1.
        /// </summary>
        private readonly SyncVar<float> _currentBalanceValue = new();

        private readonly SyncVar<int> _happyImpactCountValue = new();
        private readonly SyncVar<int> _sadImpactCountValue = new();

        public override void OnStartServer()
        {
            _currentBalanceValue.Value = 0.5f;
            _happyImpactCountValue.Value = 0;
            _sadImpactCountValue.Value = 0;
        }

        public override void OnStartClient()
        {
            SetScoreUIVisible(false);
        }

        [Server]
        public int IncrementHappyImpactCount()
        {
            _happyImpactCountValue.Value++;
            UpdateBalanceValue();
            return _happyImpactCountValue.Value;
        }

        [Server]
        public int IncrementSadImpactCount()
        {
            _sadImpactCountValue.Value++;
            UpdateBalanceValue();
            return _sadImpactCountValue.Value;
        }

        [Server]
        private void UpdateBalanceValue()
        {
            float ratio = (float)_happyImpactCountValue.Value /
                          (_happyImpactCountValue.Value + _sadImpactCountValue.Value);
            _currentBalanceValue.Value = ratio;
        }

        private void Update()
        {
            float currentUIValue = _balanceBar.value;
            float t = 1 - Mathf.Pow(0.01f, Time.deltaTime);
            float newValue = Mathf.Lerp(currentUIValue, _currentBalanceValue.Value, t);
            _balanceBar.value = newValue;

            _happyImpactCountText.text = _happyImpactCountValue.Value.ToString();
            _sadImpactCountText.text = _sadImpactCountValue.Value.ToString();
        }

        [Server]
        public void ResetScore()
        {
            _happyImpactCountValue.Value = 0;
            _sadImpactCountValue.Value = 0;
            UpdateBalanceValue();
        }

        [Client]
        public void SetScoreUIVisible(bool visible)
        {
            _scoreUI.SetActive(visible);
        }
    }
}