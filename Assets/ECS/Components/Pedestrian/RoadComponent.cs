using UnityEngine;

public struct RoadComponent
{
    public int id;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float width;
    public float moveSpeed;
}
public struct LockPuzzleStateComponent
{
    public enum Color { Red, Blue, Green }
    public Color LockColor;
    public int KeysCollected;
    public int KeysRequired;
    public bool IsActive;
    public int IsComplete;
}