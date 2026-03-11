using UnityEngine;

public class BattleBootstrap : MonoBehaviour
{
    [SerializeField] private BattleController battleController;
    [SerializeField] private EnemyData defaultEnemy;

    private void Awake()
    {
        if (battleController == null)
        {
            battleController = FindObjectOfType<BattleController>();
        }

        if (battleController == null)
        {
            Debug.LogError("BattleBootstrap: BattleController not found in scene.");
            return;
        }

        EnemyData enemyToLoad = ResolveEnemyFromSession();
        if (enemyToLoad == null)
        {
            enemyToLoad = defaultEnemy;
        }

        if (enemyToLoad != null)
        {
            battleController.currentEnemy = enemyToLoad;
        }
    }

    private EnemyData ResolveEnemyFromSession()
    {
        if (GameSession.Instance == null || string.IsNullOrEmpty(GameSession.Instance.selectedEnemyId))
        {
            return null;
        }

        EnemyData[] allEnemies = Resources.FindObjectsOfTypeAll<EnemyData>();
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i] != null && allEnemies[i].id == GameSession.Instance.selectedEnemyId)
            {
                return allEnemies[i];
            }
        }

        return null;
    }
}

