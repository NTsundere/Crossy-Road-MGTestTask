using System;

public static class PuzzleActions
{
    public static event Action BattleEnded;
    public static event Action ChestTouched;
    public static event Action PuzzleSolved;

    public static void OnBattleEnded() => BattleEnded?.Invoke();
    public static void OnChestTouched() => ChestTouched?.Invoke();
    public static void OnPuzzleSolved() => PuzzleSolved?.Invoke();

    public static void ClearAllSubscriptions()
    {
        BattleEnded = null;
        ChestTouched = null;
        PuzzleSolved = null;
    }
}
