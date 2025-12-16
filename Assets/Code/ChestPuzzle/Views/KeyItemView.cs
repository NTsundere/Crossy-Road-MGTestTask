using ChestPuzzle.Models;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChestPuzzle.Views
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup), typeof(Image))]
    public class KeyItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<KeyItemView> OnDragStarted;
        public event Action<KeyItemView, Vector2> OnDragEnded;

        public KeyColor Color { get; private set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }

        [SerializeField] private Image _keyImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private RectTransform _rectTransform;
        private Vector2 _originalPosition;
        private Vector2 _dragOffset;
        private bool _isDraggable = true;
        private bool _isInitialized = false;

        #region Unity Lifecycle

        private void Awake()
        {
            EnsureComponents();
        }

        private void EnsureComponents()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            if (_keyImage == null)
                _keyImage = GetComponent<Image>();

            if (_rectTransform != null)
                _originalPosition = _rectTransform.anchoredPosition;
        }

        #endregion

        #region Public Methods

        public void Initialize(int gridX, int gridY, KeyColor color, Color unityColor)
        {
            if (!_isInitialized)
            {
                EnsureComponents();
                _isInitialized = true;
            }

            GridX = gridX;
            GridY = gridY;
            Color = color;

            if (_keyImage != null)
            {
                _keyImage.color = unityColor;
            }

            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition = _originalPosition;
            }

            gameObject.SetActive(true);
            SetDraggable(true);

            gameObject.name = $"Key_{gridX}_{gridY}_{color}";
        }

        public void SetDraggable(bool draggable)
        {
            _isDraggable = draggable;

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = draggable;
                _canvasGroup.alpha = draggable ? 1f : 0.5f;
            }
        }

        #endregion

        #region Drag & Drop Handlers

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isDraggable) return;
            if (_rectTransform == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);

            _dragOffset = _rectTransform.anchoredPosition - localPoint;

            if (_canvasGroup != null)
                _canvasGroup.alpha = 0.7f;

            transform.SetAsLastSibling();

            OnDragStarted?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDraggable) return;
            if (_rectTransform == null) return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            {
                _rectTransform.anchoredPosition = localPoint + _dragOffset;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDraggable) return;
            if (_rectTransform == null) return;

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            OnDragEnded?.Invoke(this, eventData.position);
        }

        #endregion

        #region Helper Methods

        public void ResetPosition()
        {
            if (_rectTransform != null)
                _rectTransform.anchoredPosition = _originalPosition;
        }

        public void SetUsed()
        {
            SetDraggable(false);
            gameObject.SetActive(false);
        }

        #endregion
    }
}