using System;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class GameplayPanel : MonoBehaviour, IView
    {
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _skipLvlBtn;
        [SerializeField] private Button _undoBtn;
        [SerializeField] private TextMeshProUGUI _lvlTxt;

        public event Action RestartButtonClicked = delegate {  };
        public event Action SkipButtonClicked = delegate {  };
        public event Action UndoButtonClicked = delegate {  };

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetLevelIndex(int levelCount) => _lvlTxt.text = $"Level {levelCount}";

        private void OnEnable()
        {
            
            _restartBtn.onClick.AddListener(OnClickRestart);
            _skipLvlBtn.onClick.AddListener(OnClickNextLevel);
            _undoBtn.onClick.AddListener(OnClickUndo);
        }

        private void OnDisable()
        {
            _restartBtn.onClick.RemoveListener(OnClickRestart);
            _skipLvlBtn.onClick.RemoveListener(OnClickNextLevel);
            _undoBtn.onClick.RemoveListener(OnClickUndo);
        }

        public void UndoButtonStateChanged(bool state)
        {
            _undoBtn.interactable = state;
        }
        
        private void OnClickUndo()
        {
            UndoButtonClicked.Invoke();
        }

        private void OnClickNextLevel()
        {
            SkipButtonClicked.Invoke();
        }

        private void OnClickRestart() => RestartButtonClicked.Invoke();
    }
}
