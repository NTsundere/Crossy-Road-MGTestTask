using ChestPuzzle.Interfaces;
using ChestPuzzle.Models;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChestPuzzle.Views
{
    public class ChestPuzzleView : MonoBehaviour, IChestPuzzleView
    {
        public event Action<KeyColor, Vector2Int> OnKeyDraggedToLock;
        public event Action OnViewClosed;
        public event Action OnViewInitialized;

        [Header("Основные элементы")]
        [SerializeField] private GameObject _puzzlePanel;
        [SerializeField] private Image _lockImage;
        [SerializeField] private TMP_Text _keysCounterText;
        [SerializeField] private GridLayoutGroup _keysGrid;
        [SerializeField] private GameObject _keyPrefab;
        [SerializeField] private GameObject _winScreenPanel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _closeButton;

        [Header("Настройки")]
        [SerializeField] private float _keyDragScale = 1.2f;
        [SerializeField] private float _keyDragAlpha = 0.7f;
        [SerializeField] private float _keyDropAnimationDuration = 0.3f;

        [Header("Цвета ключей")]
        [SerializeField] private Color _redColor = Color.red;
        [SerializeField] private Color _blueColor = Color.blue;
        [SerializeField] private Color _greenColor = Color.green;
        [SerializeField] private Color _yellowColor = Color.yellow;
        [SerializeField] private Color _purpleColor = new Color(0.5f, 0f, 0.5f);
        [SerializeField] private Color _orangeColor = new Color(1f, 0.5f, 0f);

        private Dictionary<Vector2Int, KeyItemView> _keyViews = new Dictionary<Vector2Int, KeyItemView>();
        private Canvas _canvas;
        private RectTransform _lockRectTransform;
        private KeyItemView _currentDraggedKey;
        private Vector2 _dragOffset;
        private Vector3 _defaultKeyScale;

        #region Unity Lifecycle

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _lockRectTransform = _lockImage.GetComponent<RectTransform>();
            _defaultKeyScale = _keyPrefab.transform.localScale;

            Hide();

            if (_restartButton != null)
                _restartButton.onClick.AddListener(HandleRestartClick);

            if (_closeButton != null)
                _closeButton.onClick.AddListener(HandleCloseClick);
        }

        #endregion

        #region IChestPuzzleView Implementation

        public void Show()
        {
            if (_puzzlePanel != null)
            {
                _puzzlePanel.SetActive(true);
                ResetAllKeys();
            }

            if (_winScreenPanel != null)
                _winScreenPanel.SetActive(false);
        }

        public void Hide()
        {
            if (_puzzlePanel != null)
                _puzzlePanel.SetActive(false);

            if (_winScreenPanel != null)
                _winScreenPanel.SetActive(false);
        }

        public void Initialize(KeyColor lockColor, KeyColor[,] keyGrid)
        {
            ClearGrid();
            SetLockColor(lockColor);
            CreateKeyGrid(keyGrid);
            UpdateKeysCounter(0, 3);

            OnViewInitialized?.Invoke();
        }

        public void UpdateKeysCounter(int collected, int required)
        {
            if (_keysCounterText != null)
            {
                _keysCounterText.text = $"{collected}/{required}";
                
                if (collected > 0)
                {
                    _keysCounterText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
                }

                _keysCounterText.color = collected >= required ? Color.green : Color.white;
            }
        }

        public void ShowKeyResult(KeyColor keyColor, bool isCorrect, Vector2Int gridPosition)
        {
            if (_keyViews.TryGetValue(gridPosition, out var keyView))
            {
                if (isCorrect)
                {
                    PlayKeyCollectAnimation(keyView);
                }
                else
                {
                    PlayKeyErrorAnimation(keyView);
                }
            }
        }

        public void ShowWinScreen()
        {
            if (_winScreenPanel != null)
            {
                _winScreenPanel.SetActive(true);
                _puzzlePanel.SetActive(false);

                Vector3 defaultWinScreenScale = _winScreenPanel.transform.localScale;
                _winScreenPanel.transform.localScale = Vector3.zero;
                _winScreenPanel.transform.DOScale(defaultWinScreenScale, 0.5f)
                    .SetEase(Ease.OutBack);
            }
        }

        public void ShowError(string message)
        {
        }

        #endregion

        #region Grid Management

        private void ClearGrid()
        {
            foreach (var kvp in _keyViews)
            {
                if (kvp.Value != null)
                    Destroy(kvp.Value.gameObject);
            }
            _keyViews.Clear();

            if (_keysGrid != null)
            {
                for (int i = _keysGrid.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(_keysGrid.transform.GetChild(i).gameObject);
                }
            }
        }

        private void CreateKeyGrid(KeyColor[,] keyGrid)
        {
            if (_keysGrid == null || _keyPrefab == null) return;

            int rows = keyGrid.GetLength(0);
            int cols = keyGrid.GetLength(1);

            _keysGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _keysGrid.constraintCount = rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    CreateKeyItem(x, y, keyGrid[y, x]);
                }
            }
        }

        private void CreateKeyItem(int x, int y, KeyColor color)
        {

            GameObject keyObj = Instantiate(_keyPrefab, _keysGrid.transform);

            keyObj.SetActive(true);

            KeyItemView keyView = keyObj.GetComponent<KeyItemView>();

            Color unityColor = GetUnityColor(color);
            keyView.Initialize(x, y, color, unityColor);

            keyView.OnDragStarted += HandleKeyDragStarted;
            keyView.OnDragEnded += HandleKeyDragEnded;

            var gridPos = new Vector2Int(x, y);
            _keyViews[gridPos] = keyView;

            keyObj.name = $"Key_{x}_{y}_{color}";
        }

        private void ResetAllKeys()
        {
            foreach (var keyView in _keyViews.Values)
            {
                if (keyView != null)
                {
                    keyView.gameObject.SetActive(true);
                }
            }
        }

        #endregion

        #region Drag & Drop Handlers

        private void HandleKeyDragStarted(KeyItemView keyView)
        {
            _currentDraggedKey = keyView;

            keyView.transform.localScale *= _keyDragScale;

            var canvasGroup = keyView.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = _keyDragAlpha;
        }

        private void HandleKeyDragEnded(KeyItemView keyView, Vector2 dropPosition)
        {
            if (_currentDraggedKey == null) return;

            var canvasGroup = keyView.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            if (IsPositionOverLock(dropPosition))
            {
                OnKeyDraggedToLock?.Invoke(keyView.Color, new Vector2Int(keyView.GridX, keyView.GridY));
            }
            else
            {
                ReturnKeyToGrid(keyView);
            }

            _currentDraggedKey = null;
        }

        private bool IsPositionOverLock(Vector2 screenPosition)
        {
            if (_lockRectTransform == null) return false;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _lockRectTransform,
                screenPosition,
                _canvas.worldCamera,
                out Vector2 localPoint);

            return _lockRectTransform.rect.Contains(localPoint);
        }

        private async void ReturnKeyToGrid(KeyItemView keyView)
        {
            await keyView.transform.DOLocalMove(Vector3.zero, _keyDropAnimationDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask();

            await keyView.transform.DOScale(_defaultKeyScale, _keyDropAnimationDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask();
        }

        #endregion

        #region Animation Methods

        private async void PlayKeyCollectAnimation(KeyItemView keyView)
        {
            if (_lockImage != null)
            {
                var startPos = keyView.transform.position;
                var endPos = _lockImage.transform.position;

                await keyView.transform.DOMove(endPos, 0.5f)
                    .SetEase(Ease.InQuad)
                    .ToUniTask();

                await keyView.transform.DOScale(Vector3.zero, 0.2f)
                    .ToUniTask();
            }

            keyView.gameObject.SetActive(false);
        }

        private async void PlayKeyErrorAnimation(KeyItemView keyView)
        {
            await keyView.transform.DOShakePosition(0.5f, strength: 15f, vibrato: 10)
                .ToUniTask();

            await keyView.transform.DOScale(_defaultKeyScale, _keyDropAnimationDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask();
        }

        private void SetLockColor(KeyColor color)
        {
            if (_lockImage != null)
            {
                _lockImage.color = GetUnityColor(color);
                Vector3 defaultLockScale = _lockImage.transform.localScale;
                _lockImage.transform.localScale = Vector3.zero;
                _lockImage.transform.DOScale(defaultLockScale, 0.5f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.2f);
            }
        }

        #endregion

        #region Button Handlers

        private void HandleRestartClick()
        {
            Hide();
            OnViewClosed?.Invoke();
            SceneManager.LoadScene(Scenes.Main);
        }

        private void HandleCloseClick()
        {
            Hide();
            OnViewClosed?.Invoke();
        }

        #endregion

        #region Helper Methods

        private Color GetUnityColor(KeyColor keyColor)
        {
            switch (keyColor)
            {
                case KeyColor.Red: return _redColor;
                case KeyColor.Blue: return _blueColor;
                case KeyColor.Green: return _greenColor;
                case KeyColor.Yellow: return _yellowColor;
                case KeyColor.Purple: return _purpleColor;
                case KeyColor.Orange: return _orangeColor;
                default: return Color.white;
            }
        }

        #endregion
    }
}