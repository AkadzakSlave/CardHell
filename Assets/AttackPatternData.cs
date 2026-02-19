using UnityEngine;

[CreateAssetMenu(fileName = "AttackPatternData", menuName = "CardHell/Attack Pattern Data")]
public class AttackPatternData : ScriptableObject
{
    public string id;
    public PatternType type;
    public float waveDuration = 6f;
    public float spawnInterval = 0.15f;
    public float bulletSpeed = 6f;
    public int bulletDamage = 1;
    public int bulletsPerSpawn = 6;
    public float extraA;
    public float extraB;
}
