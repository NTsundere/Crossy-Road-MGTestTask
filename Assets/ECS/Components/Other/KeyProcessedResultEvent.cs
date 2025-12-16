using Leopotam.Ecs;

public struct KeyProcessedResultEvent
{
    public EcsEntity KeyEntity;
    public EcsEntity PuzzleEntity;
    public bool IsCorrect;           
    public int NewKeysCount;    
}
