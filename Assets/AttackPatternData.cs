using UnityEngine;

[CreateAssetMenu(fileName = "AttackPatternData", menuName = "Battle/Attack Pattern Data")]
public class AttackPatternData : ScriptableObject
{
    public string id = "Rain";
    public float waveDuration = 4f;
    public float spawnInterval = 0.3f;
    public float bulletSpeed = 5f;
    public int bulletsPerSpawn = 8;
    public float extraParamA = 0f;
    public float extraParamB = 0f;
}
