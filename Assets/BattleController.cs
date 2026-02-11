using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAttackSpawner enemyAttackSpawner;

    [Header("Battle State")]
    [Range(0, 100)]
    [SerializeField] private int friendship;
    [SerializeField] private bool battleActive;

    [Header("Events")]
    [SerializeField] private UnityEvent onBattleWon;
    [SerializeField] private UnityEvent onBattleLost;

    public int Friendship => friendship;
    public bool BattleActive => battleActive;

    public void StartBattle()
    {
        friendship = 0;
        battleActive = true;

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
        if (!battleActive)
        {
            return;
        }

        friendship = Mathf.Clamp(friendship + 30, 0, 100);
        Debug.Log($"Friendship: {friendship}/100");

        if (friendship >= 100)
        {
            WinBattle();
        }
    }

    public void OnPlayerDied()
    {
        if (!battleActive)
        {
            return;
        }

        battleActive = false;
        enemyAttackSpawner?.StopAttacks();
        Debug.Log("Battle lost: player died.");
        onBattleLost?.Invoke();
    }

    private void WinBattle()
    {
        battleActive = false;
        enemyAttackSpawner?.StopAttacks();
        Debug.Log("Battle won: friendship reached 100.");
        onBattleWon?.Invoke();
    }
}
