using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuPanel : MonoBehaviour, IView
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button rateButton;

        public event Action StartButtonClicked;
        public event Action RateButtonClicked;

        public void OnEnable()
        {
            startButton.onClick.AddListener(HandleStartButtonClicked);
            rateButton.onClick.AddListener(HandleRateButtonClicked);
        }

        public void OnDisable()
        {
            startButton.onClick.RemoveListener(HandleStartButtonClicked);
            rateButton.onClick.RemoveListener(HandleRateButtonClicked);
        }

        private void HandleStartButtonClicked() => StartButtonClicked?.Invoke();
        private void HandleRateButtonClicked() => RateButtonClicked?.Invoke();

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}