using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundButton: MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _soundEnableAndDisableSprites;
    
    private void Awake() => AudioManagerOnSoundStateChanged(AudioManager.IsSoundEnable);

    private void OnEnable()
    {
        _button.onClick.AddListener(SwitchAudio);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(SwitchAudio);
    }

    private void SwitchAudio()
    {
        AudioManager.IsSoundEnable = !AudioManager.IsSoundEnable;
        AudioManagerOnSoundStateChanged(AudioManager.IsSoundEnable);
    }

    private void AudioManagerOnSoundStateChanged(bool isEnabled)
    {
        _image.sprite = _soundEnableAndDisableSprites[isEnabled ? 0 : 1];
    }
}