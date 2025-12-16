using UnityEngine;

public struct PedestrianComponent
{
    public Transform transform;
    public int roadId;
    public float progress;
    public float moveSpeed;
    public bool movingRight;
    public float laneOffset;
}
