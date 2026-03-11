using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "CardHell/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite enemySprite;
    public int maxHP;
    public int coinsReward = 5;
    public List<AttackPatternData> patterns = new List<AttackPatternData>();
    public List<ActOptionData> actOptions = new List<ActOptionData>();
}
