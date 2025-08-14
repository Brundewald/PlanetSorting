using System;
using Gameplay.LevelsData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelTileUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _txt;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _completeSprite;
    [SerializeField] private Sprite _lockSprite;

    private ViewModel _mViewModel;

    public ViewModel MViewModel
    {
        get => _mViewModel;
        set
        {
            _txt.text = value.Level.no.ToString();
            _image.sprite = value.Completed ? _completeSprite : _defaultSprite;
            
            if(value.Locked)
                _image.sprite = _lockSprite;
            
            _mViewModel = value;
        }
    }
    
    public event Action<LevelTileUI> Clicked;
    
    public void OnPointerClick(PointerEventData eventData) => Clicked?.Invoke(this);

    public struct ViewModel
    {
        public Level Level { get; set; }
        public bool Locked { get; set; }
        public bool Completed { get; set; }
    }
}