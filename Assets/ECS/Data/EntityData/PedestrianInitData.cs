using UnityEngine;

[CreateAssetMenu(fileName = "InitData", menuName = "InitData/PedestrianInitData")]
public class PedestrianInitData : ScriptableObject
{
    public GameObject pedestrianPrefab;

    public static PedestrianInitData LoadFromAssets()
    {
        return Resources.Load("Data/PedestrianInitData") as PedestrianInitData;
    }
}
