using Gameplay;
using Gameplay.LevelsData;
using UI;
using UnityEngine;

namespace MyGame
{

    public class Root : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private LevelManager _levelManager;

        public static int TOTAL_GAME_COUNT
        {
            get => PrefManager.GetInt(nameof(TOTAL_GAME_COUNT));
            set => PrefManager.SetInt(nameof(TOTAL_GAME_COUNT),value);
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            _uiManager.LevelSelected += HandleLevelSelected;
            _uiManager.RateButtonClicked += HandleRateButtonClicked;
            _uiManager.LevelSkipped += HandleLevelSkipped;
            _uiManager.ContinueButtonClicked += HandleContinueButtonClicked;
            _uiManager.RestartButtonClicked += _levelManager.RestartGame;
            _uiManager.UndoButtonClicked += _levelManager.PerformUndo;
            _uiManager.StateChanged += HandleStateChanged;
            _levelManager.UndoChaged += _uiManager.UndoButtonStateChanged;
            _levelManager.LevelCompleted += HandleLevelCompleted;
            _uiManager.SwitchToState(UIState.MainMenu, false);
        }

        private void OnDisable()
        {
            _uiManager.LevelSelected -= HandleLevelSelected;
            _uiManager.RateButtonClicked -= HandleRateButtonClicked;
            _uiManager.ContinueButtonClicked -= HandleContinueButtonClicked;
            _uiManager.LevelSkipped -= HandleLevelSkipped;
            _uiManager.RestartButtonClicked -= _levelManager.RestartGame;
            _uiManager.UndoButtonClicked -= _levelManager.PerformUndo;
            _uiManager.StateChanged -= HandleStateChanged;
            _levelManager.UndoChaged -= _uiManager.UndoButtonStateChanged;
            _levelManager.LevelCompleted -= HandleLevelCompleted;
        }

        private void HandleContinueButtonClicked()
        {
            LoadNextLevel();
            _uiManager.SwitchToState(UIState.Gameplay, false);
        }

        private void HandleLevelSkipped()
        {
            //TODO add rewarded ad for skip
            ResourceManager.CompleteLevel(_levelManager.GameMode, _levelManager.Level.no);
            HandleLevelCompleted();
        }


        private void HandleLevelSelected(LevelData levelData)
        {
            _uiManager.SwitchToState(UIState.Gameplay);
            StartGame(levelData);
        }


        private void HandleLevelCompleted() => _uiManager.SwitchToState(UIState.LevelCompleted, false);

        private void LoadNextLevel()
        {
            _levelManager.DestroyLevel();
            var gameMode = _levelManager.GameMode;
            var levelNo = _levelManager.Level.no;
            var levelData = new LevelData()
            {
                Level = ResourceManager.GetLevel(gameMode, levelNo + 1),
                Difficulty = gameMode
            };
            StartGame(levelData);
        }

        private void StartGame(LevelData levelData)
        {
            _uiManager.SetLevelIndex(levelData.Level.no);
            _levelManager.StartGame(levelData);
        }

        private void HandleStateChanged(UIState currentState)
        {
            if(_levelManager.CurrentGameState == GameState.Playing && currentState != UIState.Gameplay)
                _levelManager.DestroyLevel();
        }

        private void HandleRateButtonClicked()
        {
            Debug.Log("Rate");
        }
    }
}

public struct LevelData
{
    public Difficulty Difficulty { get; set; }
    public Level Level { get; set; }
}