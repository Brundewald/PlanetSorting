using System;
using System.Collections.Generic;
using UI.Gameplay;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel _mainMenuPanel;
        [SerializeField] private DifficultyPanel _difficultyPanel;
        [SerializeField] private LevelSelectionPanel _levelSelectionPanel;
        [SerializeField] private GameplayPanel _gameplayPanel;
        [SerializeField] private LevelCompletePanel _levelCompletePanel;
        [SerializeField] private PopUpPanel _popUpPanel;
        [SerializeField] private Button _backButton;

        private readonly Stack<UIState> _statesHistory = new Stack<UIState>();
        private UIState _activeState;
        private Dictionary<UIState, IView> _views;
        
        public event Action<LevelData> LevelSelected = delegate {  };
        public event Action UndoButtonClicked = delegate {  };
        public event Action RestartButtonClicked = delegate {  };
        public event Action RateButtonClicked = delegate {  };
        public event Action LevelSkipped = delegate {  };
        public event Action ContinueButtonClicked = delegate {  };
        public event Action<UIState> StateChanged = delegate {  };
        
        public void Awake()
        {
            _backButton.gameObject.SetActive(false);
            _views = new Dictionary<UIState, IView>()
            {
                { UIState.MainMenu, _mainMenuPanel },
                { UIState.DifficultySelection, _difficultyPanel },
                { UIState.LevelSelection, _levelSelectionPanel },
                { UIState.Gameplay, _gameplayPanel },
                { UIState.LevelCompleted, _levelCompletePanel },
                { UIState.PopUp, _popUpPanel }
            };
        }

        private void OnEnable()
        {
            _mainMenuPanel.StartButtonClicked += ShowDifficultySelection;
            _mainMenuPanel.RateButtonClicked += HandleRateButtonClicked;
            _difficultyPanel.DifficultySelected += HandleDifficultySelected;
            _levelSelectionPanel.LevelSelected += HandleLevelSelected;
            _gameplayPanel.UndoButtonClicked += HandleUndoButtonClicked;
            _gameplayPanel.RestartButtonClicked += HandleRestartButtonClicked;
            _gameplayPanel.SkipButtonClicked += HandleLevelSkipped;
            _levelCompletePanel.ContinueButtonClicked += HandleContinueButtonClicked;
            _backButton.onClick.AddListener(HandleBackButtonClicked);
        }

        private void OnDisable()
        {
            _mainMenuPanel.StartButtonClicked -= ShowDifficultySelection;
            _mainMenuPanel.RateButtonClicked -= HandleRateButtonClicked;
            _difficultyPanel.DifficultySelected -= HandleDifficultySelected;
            _levelSelectionPanel.LevelSelected -= HandleLevelSelected;
            _gameplayPanel.UndoButtonClicked -= HandleUndoButtonClicked;
            _gameplayPanel.RestartButtonClicked -= HandleRestartButtonClicked;
            _gameplayPanel.SkipButtonClicked -= HandleLevelSkipped;
            _levelCompletePanel.ContinueButtonClicked -= HandleContinueButtonClicked;
            _backButton.onClick.RemoveListener(HandleBackButtonClicked);
        }

        public void SwitchToState(UIState state, bool saveHistory = true)
        {
            if(saveHistory) 
                AddToHistory(_activeState);


            foreach (var (uiState, view) in _views)
            {
                if(state == uiState)
                    view.Show();
                else 
                    view.Hide();
            }

            _activeState = state;
            StateChanged.Invoke(_activeState);
        }

        private void AddToHistory(UIState state)
        {
            if (state == UIState.None || _statesHistory.Contains(state))
                return;
            
            _statesHistory.Push(_activeState);
            UpdateBackButtonState();
        }


        public void UndoButtonStateChanged(bool state) => _gameplayPanel.UndoButtonStateChanged(state);

        public void SetLevelIndex(int index)
        {
            _levelCompletePanel.SetLevelIndex(index);
            _gameplayPanel.SetLevelIndex(index);
        }

        private void ShowDifficultySelection() => SwitchToState(UIState.DifficultySelection);

        private void HandleDifficultySelected(Difficulty difficulty)
        {
            _levelSelectionPanel.Difficulty = difficulty;
            SwitchToState(UIState.LevelSelection);
        }

        private void HandleBackButtonClicked()
        {
            var state = _statesHistory.Pop();
            SwitchToState(state, false);
            UpdateBackButtonState();
        }

        private void UpdateBackButtonState() 
            => _backButton.gameObject.SetActive(_statesHistory.Count > 0);

        private void HandleLevelSkipped() => LevelSkipped.Invoke();

        private void HandleRestartButtonClicked() => RestartButtonClicked.Invoke();

        private void HandleLevelSelected(LevelData levelData) => LevelSelected.Invoke(levelData);

        private void HandleRateButtonClicked() => RateButtonClicked.Invoke();

        private void HandleUndoButtonClicked() => UndoButtonClicked.Invoke();

        private void HandleContinueButtonClicked() => ContinueButtonClicked.Invoke();
    }

    public enum UIState
    {
        None,
        MainMenu,
        DifficultySelection,
        LevelSelection,
        Gameplay,
        LevelCompleted,
        PopUp
    }
}