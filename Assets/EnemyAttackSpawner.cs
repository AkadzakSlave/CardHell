using System.Collections;
using UnityEngine;

public class EnemyAttackSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private PlayerHeartController player;

    [Header("Rain Pattern")]
    [SerializeField] private float spawnInterval = 0.15f;
    [SerializeField] private float waveDuration = 10f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletLifeTime = 6f;
    [SerializeField] private float spawnOffsetY = 0.5f;

    private Coroutine attackRoutine;

    public void StartRainWave()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(RainRoutine());
    }

    public void StopAttacks()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private IEnumerator RainRoutine()
    {
        if (bulletPool == null)
        {
            Debug.LogError("EnemyAttackSpawner: BulletPool is not assigned.");
            yield break;
        }

        if (player == null)
        {
            player = FindFirstObjectByType<PlayerHeartController>();
            if (player == null)
            {
                Debug.LogError("EnemyAttackSpawner: PlayerHeartController not found.");
                yield break;
            }
        }

        float elapsed = 0f;
        while (elapsed < waveDuration)
        {
            SpawnRainBullet();
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        attackRoutine = null;
    }

    private void SpawnRainBullet()
    {
        Rect arena = player.GetArenaRect();
        float x = Random.Range(arena.xMin, arena.xMax);
        float y = arena.yMax + spawnOffsetY;

        Bullet bullet = bulletPool.Get();
        if (bullet == null)
        {
            return;
        }

        Rect expandedArena = Rect.MinMaxRect(arena.xMin - 1f, arena.yMin - 1f, arena.xMax + 1f, arena.yMax + 1f);
        bullet.Launch(new Vector2(x, y), Vector2.down, bulletSpeed, bulletLifeTime, expandedArena, bulletPool);
    }
}
