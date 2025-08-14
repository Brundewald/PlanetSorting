using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DifficultyPanel : MonoBehaviour, IView
    {
        [SerializeField] private Button _easyButton;
        [SerializeField] private Button _normalButton;
        [SerializeField] private Button _hardButton;

        public event Action<Difficulty> DifficultySelected = delegate {  };

        private void OnEnable()
        {
            _easyButton.onClick.AddListener(() => HandleDifficultyButtonPressed(Difficulty.Easy));
            _normalButton.onClick.AddListener(() => HandleDifficultyButtonPressed(Difficulty.Normal));
            _hardButton.onClick.AddListener(() => HandleDifficultyButtonPressed(Difficulty.Hard));
        }

        private void OnDisable()
        {
            _easyButton.onClick.RemoveAllListeners();
            _normalButton.onClick.RemoveAllListeners();
            _hardButton.onClick.RemoveAllListeners();
        }

        private void HandleDifficultyButtonPressed(Difficulty difficulty)
        {
            DifficultySelected.Invoke(difficulty);
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}