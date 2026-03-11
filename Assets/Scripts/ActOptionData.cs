using UnityEngine;

[CreateAssetMenu(fileName = "ActOptionData", menuName = "CardHell/Act Option Data")]
public class ActOptionData : ScriptableObject
{
    public string id;
    public string displayName;
    public int friendshipDelta;
    [TextArea]
    public string resultText;
}
