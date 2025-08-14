using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Gameplay
{
    public class LevelCompletePanel : MonoBehaviour, IView
    {
        [SerializeField] private TextMeshProUGUI _toastTxt;
        [SerializeField] private Button _continueBtn;
        [SerializeField] private List<string> _toasts = new List<string>();

        public event Action ContinueButtonClicked = delegate {  }; 

        public void Show()
        {
            _toastTxt.text = _toasts.GetRandom();
            gameObject.SetActive(true);
            _toastTxt.gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _toastTxt.gameObject.SetActive(false);
        }

        public void SetLevelIndex(int index)
        {
            
        }

        private void OnEnable()
        {
            _continueBtn.onClick.AddListener(OnClickContinue);
        }

        private void OnDisable()
        {
            _continueBtn.onClick.RemoveListener(OnClickContinue);
        }

        private void OnClickContinue()
        {
            ContinueButtonClicked.Invoke();
        }
    }    
}
