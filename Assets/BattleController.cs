using System;
using System.Reflection;
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
            ReturnToOverworldSafe(true, 5);
        }
    }

    public void OnPlayerDied()
    {
        ReturnToOverworldSafe(false, 0);
    }

    private void ReturnToOverworldSafe(bool isWin, int coinsReward)
    {
        MethodInfo method = FindReturnToOverworldMethod();
        if (method != null)
        {
            method.Invoke(null, new object[] { isWin, coinsReward });
            return;
        }

        Debug.LogWarning(
            $"BattleController: GameSession.ReturnToOverworld(bool,int) not found. " +
            $"Fallback log -> Win: {isWin}, Coins: {coinsReward}");
    }

    private MethodInfo FindReturnToOverworldMethod()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType("GameSession");
            if (type == null)
            {
                continue;
            }

            MethodInfo method = type.GetMethod(
                "ReturnToOverworld",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new Type[] { typeof(bool), typeof(int) },
                null);

            if (method != null)
            {
                return method;
            }
        }

        return null;
    }
}
