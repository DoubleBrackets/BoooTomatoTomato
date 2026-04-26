using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BullyController
{
    public class SelectWeaponButton : MonoBehaviour
    {
        [SerializeField]
        private string _throwableName;

        [SerializeField]
        private Button _selectButton;

        public event Action<string> OnWeaponSelected;

        private void Start()
        {
            _selectButton.onClick.AddListener(HandleSelectButtonClick);
        }

        private void OnDestroy()
        {
            _selectButton.onClick.RemoveListener(HandleSelectButtonClick);
        }

        private void HandleSelectButtonClick()
        {
            OnWeaponSelected?.Invoke(_throwableName);
        }
    }
}