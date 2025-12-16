using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace ChestPuzzle.Views
{
    public class LockView : MonoBehaviour
    {
        [SerializeField] private Image _lockImage;
        [SerializeField] private Image _lockGlow;
        [SerializeField] private float _pulseDuration = 1f;

        private Sequence _pulseSequence;

        public void SetColor(Color color)
        {
            if (_lockImage != null)
                _lockImage.color = color;
        }

        public void PlaySuccessAnimation()
        {
            _pulseSequence?.Kill();
            _pulseSequence = DOTween.Sequence();

            _pulseSequence.Append(transform.DOScale(1.2f, 0.2f));

            if (_lockGlow != null)
            {
                _pulseSequence.Join(_lockGlow.DOFade(1f, 0.2f));
            }

            _pulseSequence.Append(transform.DOScale(1f, 0.2f));

            if (_lockGlow != null)
            {
                _pulseSequence.Join(_lockGlow.DOFade(0f, 0.2f));
            }

            _pulseSequence.SetLoops(2);
        }

        public void PlayErrorAnimation()
        {
            var errorSequence = DOTween.Sequence();
            var originalColor = _lockImage.color;

            errorSequence.Append(_lockImage.DOColor(Color.red, 0.1f));
            errorSequence.Append(_lockImage.DOColor(originalColor, 0.1f));
            errorSequence.SetLoops(3);
        }

        public void ResetView()
        {
            transform.localScale = Vector3.one;

            if (_lockGlow != null)
            {
                var color = _lockGlow.color;
                color.a = 0f;
                _lockGlow.color = color;
            }

            _pulseSequence?.Kill();
        }

        private void OnDestroy()
        {
            _pulseSequence?.Kill();
        }
    }
}