using System.Collections;
using UnityEngine;

public class EnemyAttackSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private ArenaBounds arenaBounds;

    [Header("Rain Pattern")]
    [SerializeField] private float spawnInterval = 0.15f;
    [SerializeField] private float waveDuration = 10f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletLifeTime = 6f;

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
        if (bulletPool == null || arenaBounds == null)
        {
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < waveDuration)
        {
            Rect arena = arenaBounds.GetWorldRect();
            Vector2 spawn = new Vector2(Random.Range(arena.xMin, arena.xMax), arena.yMax);

            Bullet bullet = bulletPool.Get(spawn);
            if (bullet != null)
            {
                bullet.Init(
                    spawn,
                    Vector2.down * bulletSpeed,
                    bulletLifeTime,
                    bulletPool,
                    arenaBounds,
                    null,
                    1);
            }

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        attackRoutine = null;
    }
}
