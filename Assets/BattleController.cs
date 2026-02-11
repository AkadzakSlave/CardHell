using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAttackSpawner enemyAttackSpawner;

    [Header("Battle State")]
    [Range(0, 100)]
    [SerializeField] private int friendship = 0;

    public void StartBattle()
    {
        if (enemyAttackSpawner == null)
        {
            enemyAttackSpawner = FindFirstObjectByType<EnemyAttackSpawner>();
        }

        if (enemyAttackSpawner != null)
        {
            enemyAttackSpawner.StartRainWave();
        }
        else
        {
            Debug.LogError("BattleController: EnemyAttackSpawner not found.");
        }
    }

    public void OnActPressed()
    {
        friendship = Mathf.Clamp(friendship + 30, 0, 100);
        Debug.Log($"Friendship: {friendship}/100");

        if (friendship >= 100)
        {
            GameSession.ReturnToOverworld(true, 5);
        }
    }

    public void OnPlayerDied()
    {
        GameSession.ReturnToOverworld(false, 0);
    }
}
