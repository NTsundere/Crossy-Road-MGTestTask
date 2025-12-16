using Leopotam.Ecs;
using UnityEngine;

public class RhythmHitResolutionSystem : IEcsRunSystem
{
    private EcsFilter<RhythmBeatComponent> _beatFilter;
    private EcsFilter<PlayerInputEvent> _inputFilter;
    private EcsWorld _world;
    private SharedData _data;

    public void Run()
    {
        if (_data.RhytmUIHandler == null) return;

        foreach (var j in _inputFilter)
        {
            ref var input = ref _inputFilter.Get1(j);
            EcsEntity inputEntity = _inputFilter.GetEntity(j);

            float currentScale = _data.RhytmUIHandler.GetCurrentDecreasedCircleScale();
            float targetScale = _data.RhytmUIHandler.TargetScale.x;
            float startScale = _data.RhytmUIHandler.StartScale.x;

            float scaleDifference = Mathf.Abs(currentScale - targetScale);
            float totalRange = Mathf.Abs(startScale - targetScale);
            float percentageDifference = (scaleDifference / totalRange) * 100f;

            var accuracy = CalculateAccuracyByPercentage(percentageDifference);

            var resultEntity = _world.NewEntity();
            ref var result = ref resultEntity.Get<RhythmHitResultEvent>();
            result.Accuracy = accuracy;
            result.DamageMultiplier = GetDamageMultiplier(accuracy);

            inputEntity.Destroy();
        }
    }

    private RhythmBeatResultComponent.Accuracy CalculateAccuracyByPercentage(float percentageDiff)
    {
        if (percentageDiff <= 5f) return RhythmBeatResultComponent.Accuracy.Perfect;
        if (percentageDiff <= 15f) return RhythmBeatResultComponent.Accuracy.Great;
        if (percentageDiff <= 50f) return RhythmBeatResultComponent.Accuracy.Good;
        return RhythmBeatResultComponent.Accuracy.Miss;
    }
    private float GetDamageMultiplier(RhythmBeatResultComponent.Accuracy accuracy)
    {
        if (accuracy == RhythmBeatResultComponent.Accuracy.Perfect) return 2;
        if (accuracy == RhythmBeatResultComponent.Accuracy.Great) return 1.5f;
        if (accuracy == RhythmBeatResultComponent.Accuracy.Good) return 1.2f;
        return 0.5f;
    }
}
