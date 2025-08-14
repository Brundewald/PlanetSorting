using System;
using UnityEngine;

namespace UI.MainMenu
{
    public class LevelSelectionManager : MonoBehaviour, IView
    {
        [SerializeField] private DifficultyPanel _difficultyPanel;
        [SerializeField] private LevelSelectionPanel _levelSelectionPanel;
        
        public event Action<LevelData> LevelSelected = delegate { };

        public void OnEnable()
        {
            _difficultyPanel.DifficultySelected += HandleDifficultySelected;
            _levelSelectionPanel.LevelSelected += HandleLevelSelected;
            _difficultyPanel.gameObject.SetActive(true);
            _levelSelectionPanel.gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            _difficultyPanel.gameObject.SetActive(false);
            _levelSelectionPanel.gameObject.SetActive(false);
        }

        private void HandleLevelSelected(LevelData levelData) 
            => LevelSelected.Invoke(levelData);

        private void HandleDifficultySelected(Difficulty difficulty)
        {
            _levelSelectionPanel.Difficulty = difficulty;
            _difficultyPanel.gameObject.SetActive(false);
            _levelSelectionPanel.gameObject.SetActive(true);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}