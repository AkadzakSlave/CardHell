using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackDirector : MonoBehaviour
{
    [Header("References")]
    public BulletPool pool;
    public ArenaBounds arenaBounds;
    public Transform playerHeartTransform;
    public List<AttackPatternData> patterns = new List<AttackPatternData>(3);

    [Header("Bullet Defaults")]
    public int bulletDamage = 1;
    public float defaultBulletLifetime = 4f;

    private Coroutine runningRoutine;

    public void PlayRandomPattern(Action onFinished = null)
    {
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
        }

        if (patterns == null || patterns.Count == 0)
        {
            Debug.LogWarning("AttackDirector: patterns list is empty.");
            onFinished?.Invoke();
            return;
        }

        AttackPatternData selected = patterns[Random.Range(0, patterns.Count)];
        runningRoutine = StartCoroutine(PlayPatternCoroutine(selected, onFinished));
    }

    private IEnumerator PlayPatternCoroutine(AttackPatternData pattern, Action onFinished)
    {
        if (pool == null || arenaBounds == null)
        {
            Debug.LogWarning("AttackDirector: pool or arenaBounds is missing.");
            runningRoutine = null;
            onFinished?.Invoke();
            yield break;
        }

        float elapsed = 0f;
        float interval = Mathf.Max(0.01f, pattern.spawnInterval);

        while (elapsed < pattern.waveDuration)
        {
            SpawnPatternTick(pattern);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        runningRoutine = null;
        onFinished?.Invoke();
    }

    private void SpawnPatternTick(AttackPatternData pattern)
    {
        string id = pattern.id == null ? string.Empty : pattern.id.ToLowerInvariant();

        if (id.Contains("rain"))
        {
            SpawnRain(pattern);
            return;
        }

        if (id.Contains("homing"))
        {
            SpawnHoming(pattern);
            return;
        }

        if (id.Contains("pointburst") || id.Contains("burst"))
        {
            SpawnPointBurst(pattern);
            return;
        }

        // Fallback by index if IDs are not configured.
        int idx = patterns.IndexOf(pattern);
        if (idx == 0) SpawnRain(pattern);
        else if (idx == 1) SpawnHoming(pattern);
        else SpawnPointBurst(pattern);
    }

    private void SpawnRain(AttackPatternData pattern)
    {
        Rect r = arenaBounds.GetWorldRect();
        Vector2 spawn = new Vector2(Random.Range(r.xMin, r.xMax), r.yMax);
        Vector2 velocity = Vector2.down * pattern.bulletSpeed;
        SpawnBullet(spawn, velocity, pattern);
    }

    private void SpawnHoming(AttackPatternData pattern)
    {
        Rect r = arenaBounds.GetWorldRect();
        Vector2 spawn = GetRandomPointOnBorder(r);

        Vector2 target = playerHeartTransform != null
            ? (Vector2)playerHeartTransform.position
            : r.center;

        Vector2 dir = (target - spawn).normalized;
        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = Vector2.down;
        }

        SpawnBullet(spawn, dir * pattern.bulletSpeed, pattern);
    }

    private void SpawnPointBurst(AttackPatternData pattern)
    {
        Rect r = arenaBounds.GetWorldRect();
        Vector2 center = new Vector2(Random.Range(r.xMin, r.xMax), Random.Range(r.yMin, r.yMax));

        int count = Mathf.Max(1, pattern.bulletsPerSpawn);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            SpawnBullet(center, dir * pattern.bulletSpeed, pattern);
        }
    }

    private Vector2 GetRandomPointOnBorder(Rect r)
    {
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: return new Vector2(Random.Range(r.xMin, r.xMax), r.yMax); // top
            case 1: return new Vector2(Random.Range(r.xMin, r.xMax), r.yMin); // bottom
            case 2: return new Vector2(r.xMin, Random.Range(r.yMin, r.yMax)); // left
            default: return new Vector2(r.xMax, Random.Range(r.yMin, r.yMax)); // right
        }
    }

    private void SpawnBullet(Vector2 position, Vector2 velocity, AttackPatternData pattern)
    {
        Bullet bullet = pool.Get(position);
        if (bullet == null)
        {
            return;
        }

        float lifetime = pattern.extraParamA > 0f ? pattern.extraParamA : defaultBulletLifetime;
        bullet.Init(position, velocity, bulletDamage, lifetime, pool, arenaBounds);
    }
}
