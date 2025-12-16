using Leopotam.Ecs;
using System.Threading;
using UnityEngine;

public class UISyncSystem : IEcsRunSystem
{
    private EcsFilter<StartBeatAnimationEvent> _beatFilter;
    private EcsFilter<ShowHitResultUIEvent> _resultFilter;

    private SharedData _data;

    public UISyncSystem(SharedData data)
    {
        _data = data;
    }

    public void Run()
    {
        foreach (var i in _beatFilter)
        {
            ref var beatEvent = ref _beatFilter.Get1(i);

            _data.RhytmUIHandler.StartBeatAnimation(beatEvent.BeatDuration);

            _beatFilter.GetEntity(i).Destroy();

        }

        foreach (var i in _resultFilter)
        {
            ref var result = ref _resultFilter.Get1(i);

            Color accuracyColor = GetAccuracyColor(result.Accuracy);

            _data.RhytmUIHandler.ShowHitResult(result.Accuracy, accuracyColor);

            _resultFilter.GetEntity(i).Destroy();
        }
    }

    private Color GetAccuracyColor(RhythmBeatResultComponent.Accuracy accuracy)
    {
        if (accuracy == RhythmBeatResultComponent.Accuracy.Perfect) return Color.green;
        if (accuracy == RhythmBeatResultComponent.Accuracy.Great) return Color.blue;
        if (accuracy == RhythmBeatResultComponent.Accuracy.Good) return Color.yellow;
        if (accuracy == RhythmBeatResultComponent.Accuracy.Miss) return Color.red;

        return Color.white;
    }
}
