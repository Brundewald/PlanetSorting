using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.LevelsData;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private Camera _camera;
        [SerializeField] private Holder _holderPrefab;
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private AudioClip _winClip;
        [SerializeField] private RawImageRenderTextureUtility _renderTextureUtility;
        [Space]
        [SerializeField] private int _maxHolderInRowCount = 5;
        [SerializeField] private float _minXDistanceBetweenHolders = 2;
        [SerializeField] private float _maxXDistanceBetweenHolders = 4;
        [SerializeField] private float _verticalRowDistance = 6;

        private readonly List<Holder> _holders = new List<Holder>();
        private readonly Stack<MoveData> _undoStack = new Stack<MoveData>();
        private bool _haveUndo;
        private readonly int targetResolution = 1024;

        public GameState CurrentGameState { get; private set; } = GameState.None;
        public Difficulty GameMode { get; private set; } = Difficulty.Easy;
        public Level Level { get; private set; }
    
        private bool HaveUndo
        {
            set
            {
                if (value != _haveUndo)
                    _haveUndo = value;
                
                UndoChaged.Invoke(value);
            }
            get => _haveUndo;
        }
    
        public event Action<bool> UndoChaged = delegate { };
        public event Action LevelCompleted = delegate { };
    
    
        public void StartGame(LevelData levelData)
        {
            GameMode = levelData.Difficulty;
            Level = levelData.Level;
            LoadLevel();
            CurrentGameState = GameState.Playing;
        }
    
        public void RestartGame()
        {
            DestroyLevel();
            var levelData = new LevelData {Difficulty = GameMode, Level = Level};
            StartGame(levelData);
        }
    
        public void DestroyLevel()
        {
            CurrentGameState = GameState.None;
            foreach (var holder in _holders)
            {
                Destroy(holder.gameObject);
            }
            _undoStack.Clear();
            _holders.Clear();
        }
    
        private void LoadLevel()
        {
            var list = PositionsForHolders(Level.map.Count, out var width).ToList();

            var resolutionModifier = _camera.targetTexture.width / targetResolution;
            _camera.orthographicSize = (0.5f * width) * resolutionModifier;
    
            for (var i = 0; i < Level.map.Count; i++)
            {
                var levelColumn = Level.map[i];
                var holder = Instantiate(_holderPrefab, list[i], Quaternion.identity, transform);
    
                var balls = CreateBalls(levelColumn, holder.transform);
                
                holder.Init(balls);
    
                _holders.Add(holder);
            }
        }
    
        private List<Ball> CreateBalls(LevelColumn levelColumn, Transform holderTransform)
        {
            var balls = new List<Ball>();
            
            for (var i = 0; i < levelColumn.values.Count; i++)
            {
                var ball = Instantiate(_ballPrefab, holderTransform);
                ball.GroupId = levelColumn.values[i];
                balls.Add(ball);
            }
            
            return balls;
        }
    
        public void PerformUndo()
        {
            if(CurrentGameState!=GameState.Playing || _undoStack.Count<=0)
                return;
    
            var moveData = _undoStack.Pop();
            MoveBallFromOneToAnother(moveData.ToHolder,moveData.FromHolder);
        }
    
        private void Update()
        {
            if(CurrentGameState != GameState.Playing)
                return;
    
            if (Input.GetMouseButtonDown(0))
            {
                var position = _renderTextureUtility.ScreenToRenderTexPoint(Input.mousePosition);
                var collider = Physics2D.OverlapPoint(position);
                
                if (collider is null || !collider.TryGetComponent(out Holder holder))
                    return;

                OnClickHolder(holder);
            }
        }
    
        private void OnClickHolder(Holder holder)
        {
            var pendingHolder = _holders.FirstOrDefault(h => h.IsPending);
    
            if (pendingHolder != null && pendingHolder != holder)
            {
                if (holder.TopBall == null || (pendingHolder.TopBall.GroupId == holder.TopBall.GroupId && !holder.IsFull))
                {
                    _undoStack.Push(new MoveData
                    {
                        FromHolder = pendingHolder,
                        ToHolder = holder,
                        Ball = pendingHolder.TopBall
                    });
                    HaveUndo = _undoStack.Count > 0;
                    MoveBallFromOneToAnother(pendingHolder,holder);
    
                }
                else
                {
                    pendingHolder.IsPending = false;
                    holder.IsPending = true;
                }
            }
            else
            {
                if (holder.Balls.Any())
                    holder.IsPending = !holder.IsPending;
            }
        }
    
        private void MoveBallFromOneToAnother(Holder fromHolder,Holder toHolder)
        {
            toHolder.Move(fromHolder.RemoveTopBall());
            CheckAndGameOver();
        }
    
        private void CheckAndGameOver()
        {
            if (_holders.All(holder =>
            {
                var balls = holder.Balls.ToList();
                return balls.Count == 0 || balls.All(ball => ball.GroupId == balls.First().GroupId);
            }) && _holders.Where(holder => holder.Balls.Any()).GroupBy(holder => holder.Balls.First().GroupId).All(holders => holders.Count()==1)) 
            {
                OverTheGame();
            }
        }
    
        private void OverTheGame()
        {
            if(CurrentGameState!=GameState.Playing)
                return;
    
            PlayClipIfCan(_winClip);
            CurrentGameState = GameState.Over;
          
            ResourceManager.CompleteLevel(GameMode,Level.no);
            LevelCompleted?.Invoke();
        }
    
        private void PlayClipIfCan(AudioClip clip,float volume=0.35f)
        {
            if(!AudioManager.IsSoundEnable || clip ==null)
                return;
            
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position,volume);
        }
    
        public IEnumerable<Vector2> PositionsForHolders(int count, out float expectWidth)
        {
            var xDistanceBetweenHolders = _minXDistanceBetweenHolders;
            
            expectWidth = 4 * xDistanceBetweenHolders;

            if (count <= _maxHolderInRowCount)
            {
                var minPoint = transform.position - ((count - 1) / 2f) * xDistanceBetweenHolders * Vector3.right - Vector3.up * 1f;
    
                expectWidth = Mathf.Max(count * xDistanceBetweenHolders, expectWidth);
    
                return Enumerable.Range(0, count)
                    .Select(i => (Vector2) minPoint + i * xDistanceBetweenHolders * Vector2.right);
            }
            
            var maxCountInRow = Mathf.CeilToInt(count / 2f);

            if (maxCountInRow == 3)
                xDistanceBetweenHolders = _maxXDistanceBetweenHolders;
            
            if ((maxCountInRow + 1) * xDistanceBetweenHolders > expectWidth)
            {
                expectWidth = (maxCountInRow + 1) * xDistanceBetweenHolders;
            }
            
            var centerPosition = transform.position;
            var rowVerticalOffset = Vector3.up * _verticalRowDistance;
            var rowHorizontalOffset = ((maxCountInRow - 1) / 2f) * xDistanceBetweenHolders * Vector3.right ;

            var topRowMinPoint = centerPosition + rowVerticalOffset - rowHorizontalOffset - Vector3.up;
            var lowRowMinPoint = centerPosition - rowVerticalOffset - rowHorizontalOffset - Vector3.up;
            
            var list = new List<Vector2>();
            
            list.AddRange(Enumerable.Range(0, maxCountInRow)
                .Select(i => (Vector2) topRowMinPoint + i * xDistanceBetweenHolders * Vector2.right));
            list.AddRange(Enumerable.Range(0, count - maxCountInRow)
                .Select(i => (Vector2) lowRowMinPoint + i * xDistanceBetweenHolders * Vector2.right));
    
            return list;
        }
    }
}
