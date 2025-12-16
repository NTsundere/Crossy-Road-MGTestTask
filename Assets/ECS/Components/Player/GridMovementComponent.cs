using UnityEngine;

public struct GridMovementComponent
{
    public Vector2Int currentGridPos;
    public Vector2Int targetGridPos;
    public float moveTimer;
    public float moveDuration;
    public bool isMoving;
    public float jumpHeight;
    
    public Vector3 startWorldPos;
    public Vector3 targetWorldPos;
}