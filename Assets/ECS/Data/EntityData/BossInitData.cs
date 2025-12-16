using UnityEngine;

[CreateAssetMenu(fileName = "InitData", menuName = "InitData/BossInitData")]
public class BossInitData : ScriptableObject
{
    public GameObject bossPrefab;
    public float defaultAttack = 10f;

    public static BossInitData LoadFromAssets()
    {
        return Resources.Load("Data/BossInitData") as BossInitData;
    }
}
