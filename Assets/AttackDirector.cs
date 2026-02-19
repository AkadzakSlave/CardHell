using System;
using System.Collections;
using UnityEngine;

public class AttackDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool pool;
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField] private Transform playerHeartTransform;
    [SerializeField] private AttackPatternData[] availablePatterns;

    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifetime = 10f;
    [SerializeField] private float outsideSpawnOffset = 0.25f;
    [SerializeField] private float sideStreamYJitter = 0.25f;

    private Coroutine activeRoutine;


    public void PlayRandomPattern(Action onFinished = null)
    {
        AttackPatternData chosen = null;
        if (availablePatterns != null && availablePatterns.Length > 0)
        {
            int start = UnityEngine.Random.Range(0, availablePatterns.Length);
            for (int i = 0; i < availablePatterns.Length; i++)
            {
                AttackPatternData candidate = availablePatterns[(start + i) % availablePatterns.Length];
                if (candidate != null)
                {
                    chosen = candidate;
                    break;
                }
            }
        }

        PlayPattern(chosen, onFinished);
    }


    public void StopAttacks()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }
    }

    public void PlayPattern(AttackPatternData pattern, Action onFinished)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        activeRoutine = StartCoroutine(PlayPatternRoutine(pattern, onFinished));
    }

    private IEnumerator PlayPatternRoutine(AttackPatternData pattern, Action onFinished)
    {
        if (pattern == null || pool == null || !TryGetArenaRect(out _))
        {
            onFinished?.Invoke();
            activeRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        float spawnInterval = Mathf.Max(0.01f, pattern.spawnInterval);
        float duration = Mathf.Max(0f, pattern.waveDuration);

        while (elapsed < duration)
        {
            SpawnByPatternType(pattern);
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        onFinished?.Invoke();
        activeRoutine = null;
    }

    private void SpawnByPatternType(AttackPatternData pattern)
    {
        switch (pattern.type)
        {
            case PatternType.Rain:
                SpawnRain(pattern);
                break;
            case PatternType.Homing:
                SpawnHoming(pattern);
                break;
            case PatternType.PointBurst:
                SpawnPointBurst(pattern);
                break;
            case PatternType.SideStream:
                SpawnSideStream(pattern);
                break;
            default:
                SpawnRain(pattern);
                break;
        }
    }

    private void SpawnRain(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect))
        {
            return;
        }

        Vector2 spawnPos = new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            rect.yMax + outsideSpawnOffset);

        SpawnBullet(spawnPos, Vector2.down * pattern.bulletSpeed, pattern.bulletDamage);
    }

    private void SpawnHoming(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect))
        {
            return;
        }

        Vector2 spawnPos = RandomPointOnBorder(rect);
        Vector2 playerPos = playerHeartTransform != null ? (Vector2)playerHeartTransform.position : rect.center;
        Vector2 direction = (playerPos - spawnPos).normalized;
        if (direction == Vector2.zero)
        {
            direction = Vector2.down;
        }

        SpawnBullet(spawnPos, direction * pattern.bulletSpeed, pattern.bulletDamage);
    }

    private void SpawnPointBurst(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect))
        {
            return;
        }

        Vector2 center = new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            UnityEngine.Random.Range(rect.yMin, rect.yMax));

        center += new Vector2(pattern.extraA, pattern.extraB);
        center.x = Mathf.Clamp(center.x, rect.xMin, rect.xMax);
        center.y = Mathf.Clamp(center.y, rect.yMin, rect.yMax);

        int count = Mathf.Max(1, pattern.bulletsPerSpawn);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            SpawnBullet(center, direction * pattern.bulletSpeed, pattern.bulletDamage);
        }
    }

    private void SpawnSideStream(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect))
        {
            return;
        }

        bool spawnFromLeft = UnityEngine.Random.value < 0.5f;
        float y = UnityEngine.Random.Range(rect.yMin, rect.yMax) + UnityEngine.Random.Range(-sideStreamYJitter, sideStreamYJitter);
        y = Mathf.Clamp(y, rect.yMin, rect.yMax);

        Vector2 spawnPos = spawnFromLeft
            ? new Vector2(rect.xMin - outsideSpawnOffset, y)
            : new Vector2(rect.xMax + outsideSpawnOffset, y);

        Vector2 velocity = spawnFromLeft
            ? Vector2.right * pattern.bulletSpeed
            : Vector2.left * pattern.bulletSpeed;

        SpawnBullet(spawnPos, velocity, pattern.bulletDamage);
    }

    private void SpawnBullet(Vector2 position, Vector2 velocity, int damage)
    {
        Bullet bullet = pool.Get(position);
        if (bullet == null)
        {
            return;
        }

        bullet.Init(position, velocity, damage, bulletLifetime, pool, arenaBounds);
    }

    private bool TryGetArenaRect(out Rect rect)
    {
        rect = default;
        if (arenaBounds == null)
        {
            return false;
        }

        rect = arenaBounds.GetWorldRect();
        return true;
    }

    private Vector2 RandomPointOnBorder(Rect rect)
    {
        int side = UnityEngine.Random.Range(0, 4);
        switch (side)
        {
            case 0:
                return new Vector2(UnityEngine.Random.Range(rect.xMin, rect.xMax), rect.yMax + outsideSpawnOffset);
            case 1:
                return new Vector2(UnityEngine.Random.Range(rect.xMin, rect.xMax), rect.yMin - outsideSpawnOffset);
            case 2:
                return new Vector2(rect.xMin - outsideSpawnOffset, UnityEngine.Random.Range(rect.yMin, rect.yMax));
            default:
                return new Vector2(rect.xMax + outsideSpawnOffset, UnityEngine.Random.Range(rect.yMin, rect.yMax));
        }
    }
}
