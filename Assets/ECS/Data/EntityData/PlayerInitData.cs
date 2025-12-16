using UnityEngine;

[CreateAssetMenu(fileName = "InitData", menuName = "InitData/PlayerInitData")]
public class PlayerInitData : ScriptableObject
{
    public GameObject playerPrefab;
    public float defaultSpeed = 2f;

    public static  PlayerInitData LoadFromAssets()
    {
        return Resources.Load("Data/PlayerInitData") as PlayerInitData;
    }
}
