using UnityEngine;

[CreateAssetMenu(fileName = "InitData", menuName = "InitData/ChestInitData")]
public class ChestInitData : ScriptableObject
{
    public GameObject chestPrefab;

    public static ChestInitData LoadFromAssets()
    {
        return Resources.Load("Data/ChestInitData") as ChestInitData;
    }
}
