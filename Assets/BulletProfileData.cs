using UnityEngine;

[CreateAssetMenu(fileName = "BulletProfileData", menuName = "CardHell/Bullet Profile Data")]
public class BulletProfileData : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public Vector2 visualScale = Vector2.one;
    public float hitboxRadius = 0.1f;
    public Color tint = Color.white;
    public bool useTrail;
    public int baseDamage = 1;
}
