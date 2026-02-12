using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Attack Pattern Data", fileName = "AttackPatternData")]
public class AttackPatternData : ScriptableObject
{
    public string id = "Pattern";
    public float waveDuration = 4f;
    public float spawnInterval = 0.2f;
    public float bulletSpeed = 6f;
    public int bulletsPerSpawn = 8;
    public float extraParamA = 0f;
    public float extraParamB = 0f;
}
