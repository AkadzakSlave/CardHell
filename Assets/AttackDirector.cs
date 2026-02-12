using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDirector : MonoBehaviour
{
    [Header("References")]
    public BulletPool pool;
    public ArenaBounds arenaBounds;
    public Transform playerHeartTransform;
    public List<AttackPatternData> patterns = new List<AttackPatternData>(3);

    [Header("Bullet Settings")]
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float bulletLifetime = 10f;

    private Coroutine activeRoutine;

    public void PlayRandomPattern(Action onFinished = null)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }

        activeRoutine = StartCoroutine(PlayRandomPatternRoutine(onFinished));
    }

    private IEnumerator PlayRandomPatternRoutine(Action onFinished)
    {
        if (!TryGetPattern(out AttackPatternData pattern))
        {
            onFinished?.Invoke();
            activeRoutine = null;
            yield break;
        }

        string normalizedId = pattern.id == null ? string.Empty : pattern.id.Trim().ToLowerInvariant();

        if (normalizedId == "homing")
        {
            yield return RunHoming(pattern);
        }
        else if (normalizedId == "pointburst")
        {
            yield return RunPointBurst(pattern);
        }
        else
        {
            yield return RunRain(pattern);
        }

        onFinished?.Invoke();
        activeRoutine = null;
    }

    private bool TryGetPattern(out AttackPatternData pattern)
    {
        pattern = null;
        if (patterns == null || patterns.Count == 0)
        {
            return false;
        }

        List<AttackPatternData> valid = new List<AttackPatternData>();
        for (int i = 0; i < patterns.Count; i++)
        {
            if (patterns[i] != null)
            {
                valid.Add(patterns[i]);
            }
        }

        if (valid.Count == 0)
        {
            return false;
        }

        pattern = valid[UnityEngine.Random.Range(0, valid.Count)];
        return true;
    }

    private IEnumerator RunRain(AttackPatternData pattern)
    {
        float elapsed = 0f;
        while (elapsed < pattern.waveDuration)
        {
            SpawnRainBullet(pattern);
            float wait = Mathf.Max(0.01f, pattern.spawnInterval);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }
    }

    private IEnumerator RunHoming(AttackPatternData pattern)
    {
        float elapsed = 0f;
        while (elapsed < pattern.waveDuration)
        {
            SpawnHomingBullet(pattern);
            float wait = Mathf.Max(0.01f, pattern.spawnInterval);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }
    }

    private IEnumerator RunPointBurst(AttackPatternData pattern)
    {
        float elapsed = 0f;
        while (elapsed < pattern.waveDuration)
        {
            SpawnPointBurst(pattern);
            float wait = Mathf.Max(0.01f, pattern.spawnInterval);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }
    }

    private void SpawnRainBullet(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect arena))
        {
            return;
        }

        Vector2 spawn = new Vector2(UnityEngine.Random.Range(arena.xMin, arena.xMax), arena.yMax);
        SpawnBullet(spawn, Vector2.down * pattern.bulletSpeed);
    }

    private void SpawnHomingBullet(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect arena))
        {
            return;
        }

        Vector2 spawn = RandomPointOnBorder(arena);
        Vector2 target = playerHeartTransform != null ? (Vector2)playerHeartTransform.position : arena.center;
        Vector2 dir = (target - spawn).normalized;
        if (dir == Vector2.zero)
        {
            dir = Vector2.down;
        }

        SpawnBullet(spawn, dir * pattern.bulletSpeed);
    }

    private void SpawnPointBurst(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect arena))
        {
            return;
        }

        Vector2 center = new Vector2(
            UnityEngine.Random.Range(arena.xMin, arena.xMax),
            UnityEngine.Random.Range(arena.yMin, arena.yMax));

        int count = Mathf.Max(1, pattern.bulletsPerSpawn);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            SpawnBullet(center, dir * pattern.bulletSpeed);
        }
    }

    private void SpawnBullet(Vector2 position, Vector2 velocity)
    {
        if (pool == null)
        {
            return;
        }

        Bullet bullet = pool.Get(position);
        if (bullet == null)
        {
            return;
        }

        bullet.Init(position, velocity, bulletDamage, bulletLifetime, pool, arenaBounds);
    }

    private bool TryGetArenaRect(out Rect arena)
    {
        arena = default;
        if (arenaBounds == null)
        {
            return false;
        }

        arena = arenaBounds.GetWorldRect();
        return true;
    }

    private Vector2 RandomPointOnBorder(Rect rect)
    {
        int side = UnityEngine.Random.Range(0, 4);
        switch (side)
        {
            case 0:
                return new Vector2(UnityEngine.Random.Range(rect.xMin, rect.xMax), rect.yMax); // Top
            case 1:
                return new Vector2(UnityEngine.Random.Range(rect.xMin, rect.xMax), rect.yMin); // Bottom
            case 2:
                return new Vector2(rect.xMin, UnityEngine.Random.Range(rect.yMin, rect.yMax)); // Left
            default:
                return new Vector2(rect.xMax, UnityEngine.Random.Range(rect.yMin, rect.yMax)); // Right
        }
    }
}
