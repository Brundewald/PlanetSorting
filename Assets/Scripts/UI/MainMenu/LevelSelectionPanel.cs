using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.MainMenu
{
    public class LevelSelectionPanel : MonoBehaviour, IView
    {
        [SerializeField] private LevelTileUI _levelTileUIPrefab;
        [SerializeField] private RectTransform _content;

        private readonly List<LevelTileUI> _tiles = new List<LevelTileUI>();
        private Difficulty _difficulty;

        public Difficulty Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;

                var levels = ResourceManager.GetLevels(value).ToList();

                for (var i = 0; i < levels.Count; i++)
                {
                    var level = levels[i];
                    if (_tiles.Count<=i)
                    {
                        var levelTileUI = Instantiate(_levelTileUIPrefab,_content);
                        levelTileUI.Clicked +=LevelTileUIOnClicked;
                        _tiles.Add(levelTileUI);
                    }

                    _tiles[i].MViewModel = new LevelTileUI.ViewModel
                    {
                        Level = level,
                        Locked = ResourceManager.IsLevelLocked(value,level.no),
                        Completed = ResourceManager.GetCompletedLevel(value)>=level.no
                    };
                }

            }
        }

        public event Action<LevelData> LevelSelected = delegate {};

        private void LevelTileUIOnClicked(LevelTileUI tileUI)
        {
            if (tileUI.MViewModel.Locked)
                return;

            LevelSelected.Invoke(new LevelData
            {
                Difficulty = _difficulty,
                Level = tileUI.MViewModel.Level
            });
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
