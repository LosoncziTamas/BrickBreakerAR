﻿using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Gui.Modal
{
    public class PauseModal : MonoBehaviour
    {
        public enum ReturnState
        {
            Game,
            MainMenu
        }

        public Vector3 HiddenPosition;
        public float AnimDuration;
        public Ease Ease;
        
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _musicToggle;
        [SerializeField] private Button _soundToggle;
        [SerializeField] private Button _cancel;
        [SerializeField] private Canvas _tutorialCanvas;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Image _overlay;
        [SerializeField] private TextMeshProUGUI _title;

        private TaskCompletionSource<ReturnState> _taskCompletionSource;

        private void Awake()
        {
            _panel.anchoredPosition = HiddenPosition;
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClick);
            _continueButton.onClick.AddListener(DismissAndResume);
            _cancel.onClick.AddListener(DismissAndResume);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClick);
            _continueButton.onClick.RemoveListener(DismissAndResume);
            _cancel.onClick.RemoveListener(DismissAndResume);
        }

        private async void OnGUI()
        {
            GUILayout.Space(100);
            if (GUILayout.Button("Show Pause"))
            {
                await Show("I");
            }
        }

        public Task<ReturnState> Show(string stageName)
        {
            _tutorialCanvas.enabled = true;
            _title.text = $"STAGE {stageName}";
            _panel.DOLocalMove(Vector3.zero, AnimDuration).SetEase(Ease);
            _overlay.DOFade(0.5f, AnimDuration).SetEase(Ease);
            Debug.Assert(_taskCompletionSource == null);
            _taskCompletionSource = new TaskCompletionSource<ReturnState>();
            return _taskCompletionSource.Task;
        }

        private void OnHidden(ReturnState returnState)
        {
            _tutorialCanvas.enabled = false;
            _taskCompletionSource.SetResult(returnState);
            _taskCompletionSource = null;
        }
        
        private void DismissAndResume()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration)
                .SetEase(Ease)
                .OnComplete(() => OnHidden(ReturnState.Game));
        }

        private void OnBackButtonClick()
        {
            _overlay.DOFade(0.0f, AnimDuration).SetEase(Ease);
            _panel.DOLocalMove(HiddenPosition, AnimDuration)
                .SetEase(Ease)
                .OnComplete(() => OnHidden(ReturnState.MainMenu));
        }
    }
}