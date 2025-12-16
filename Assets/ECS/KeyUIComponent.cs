using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

public class KeyUIComponent : MonoBehaviour
{
    public EcsEntity KeyEntity;
    public LockPuzzleStateComponent.Color KeyColor;
    public bool IsUsed;
    public int GridIndex;
    public Image KeyImage;

    private void Awake()
    {
        KeyImage = GetComponent<Image>();

        IsUsed = false;
    }

    public Color GetUnityColor()
    {
        switch (KeyColor)
        {
            case LockPuzzleStateComponent.Color.Red: return Color.red;
            case LockPuzzleStateComponent.Color.Blue: return Color.blue;
            case LockPuzzleStateComponent.Color.Green: return Color.green;
            default: return Color.white;
        }
    }
}
