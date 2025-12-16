using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RhytmUIHandler : MonoBehaviour
{
    [SerializeField] private Image _staticCircle;
    [SerializeField] private Image _decreasedCircle;
    [SerializeField] private TMP_Text _accuracyText;

    public CancellationTokenSource AnimationCts;
    private Sequence _beatSequence;

    private Vector3 _defaultStaticCircleScale;
    private Vector3 _defaultDynamicCircleScale;

    public Vector3 StartScale => _defaultDynamicCircleScale;
    public Vector3 TargetScale => _defaultStaticCircleScale;


    private void Awake()
    {
        AnimationCts = new CancellationTokenSource();

        _defaultStaticCircleScale = _staticCircle.transform.localScale;
        _defaultDynamicCircleScale = _decreasedCircle.transform.localScale;

        _staticCircle.gameObject.SetActive(false);
        _decreasedCircle.gameObject.SetActive(false);

        PuzzleActions.BattleEnded += EndBattle;
    }

    public void StartBeatAnimation(float beatDuration)
    {
        CleanupAnimations();

        var cts = new CancellationTokenSource();

        _staticCircle.gameObject.SetActive(true);
        _decreasedCircle.gameObject.SetActive(true);
        _decreasedCircle.transform.localScale = _defaultDynamicCircleScale * 1.5f;
        _decreasedCircle.color = Color.white;

        _beatSequence = DOTween.Sequence()
            .Append(_decreasedCircle.transform.DOScale(Vector3.one, beatDuration).SetEase(Ease.Linear))
            .AppendCallback(() => _decreasedCircle.color = Color.gray)
            .OnComplete(() => cts?.Dispose())
            .SetLink(gameObject);

        this.GetCancellationTokenOnDestroy().Register(() => {
            cts?.Cancel();
            cts?.Dispose();
        });
    }

    public void ShowHitResult(RhythmBeatResultComponent.Accuracy accuracy, Color accuracyColor)
    {
        _beatSequence?.Kill();
        _decreasedCircle.transform.localScale = _defaultDynamicCircleScale;
        _decreasedCircle.gameObject.SetActive(false);

        _accuracyText.gameObject.SetActive(true);
        _accuracyText.text = accuracy.ToString();
        _accuracyText.color = accuracyColor;
        _accuracyText.alpha = 1f;

        AnimationCts?.Cancel();
        AnimationCts?.Dispose();
        AnimationCts = new CancellationTokenSource();

        PlayHitAnimationsAsync(AnimationCts.Token).Forget();
    }

    private async UniTaskVoid PlayHitAnimationsAsync(CancellationToken ct)
    {
        Vector3 defaultTextScale = _accuracyText.transform.localScale;
        try
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                ct,
                this.GetCancellationTokenOnDestroy()
            );
            await UniTask.WhenAll(
                _accuracyText.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f)
                    .SetEase(Ease.OutBack)
                    .ToUniTask(cancellationToken: linkedCts.Token),

                _accuracyText.DOFade(0f, 0.8f)
                    .SetDelay(0.3f)
                    .ToUniTask(cancellationToken: linkedCts.Token)
            );
            _accuracyText.transform.localScale = defaultTextScale;
            _accuracyText.gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _accuracyText.transform.localScale = defaultTextScale;
            _accuracyText.gameObject.SetActive(false);
            AnimationCts?.Dispose();
            AnimationCts = null;
        }
    }

    private void CleanupAnimations()
    {
        AnimationCts?.Cancel();
        AnimationCts?.Dispose();
        AnimationCts = null;
        _beatSequence?.Kill();
    }

    public void EndBattle()
    {
        CleanupAnimations();
        _decreasedCircle.transform.localScale = StartScale;
        _staticCircle.gameObject.SetActive(false);
        _decreasedCircle.gameObject.SetActive(false);
        _accuracyText.gameObject.SetActive(false);
    }

    public float GetCurrentDecreasedCircleScale()
    {
        return _decreasedCircle.transform.localScale.x;
    }

    void OnDestroy()
    {
        CleanupAnimations();

        PuzzleActions.BattleEnded -= EndBattle;
    }
}
