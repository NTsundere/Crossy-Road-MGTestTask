public struct PlayerStateComponent
{
    public enum GameState
    {
        Ready,
        Moving,
        Battle,
        Dead
    }
    public GameState currentState;
}