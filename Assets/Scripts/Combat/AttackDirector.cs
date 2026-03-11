using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BulletPool pool;
    [SerializeField] private ArenaBounds arenaBounds;
    [SerializeField] private Transform playerHeartTransform;

    [Header("Patterns")]
    [SerializeField] public List<AttackPatternData> availablePatterns = new List<AttackPatternData>();

    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifetime = 10f;
    [SerializeField] private float outsideSpawnOffset = 0.25f;

    private Coroutine activeRoutine;

    public void PlayRandomPattern(Action onFinished)
    {
        AttackPatternData chosen = null;
        int count = availablePatterns != null ? availablePatterns.Count : 0;

        if (count > 0)
        {
            int start = UnityEngine.Random.Range(0, count);
            for (int i = 0; i < count; i++)
            {
                AttackPatternData candidate = availablePatterns[(start + i) % count];
                if (candidate != null)
                {
                    chosen = candidate;
                    break;
                }
            }
        }

        PlayPattern(chosen, onFinished);
    }

    public void PlayPattern(AttackPatternData pattern, Action onFinished)
    {
        StopAttacks();
        activeRoutine = StartCoroutine(PlayPatternRoutine(pattern, onFinished));
    }

    public void StopAttacks()
    {
        if (activeRoutine == null)
        {
            return;
        }

        StopCoroutine(activeRoutine);
        activeRoutine = null;
    }

    private IEnumerator PlayPatternRoutine(AttackPatternData pattern, Action onFinished)
    {
        if (pattern == null || pool == null || !TryGetArenaRect(out _))
        {
            onFinished?.Invoke();
            activeRoutine = null;
            yield break;
        }

        float duration = Mathf.Max(0f, pattern.waveDuration);
        float spawnInterval = Mathf.Max(0.01f, pattern.spawnInterval);
        float elapsed = 0f;
        float spawnTimer = 0f;
        float spiralAngle = 0f;

        while (elapsed < duration)
        {
            spawnTimer += Time.deltaTime;
            elapsed += Time.deltaTime;

            while (spawnTimer >= spawnInterval)
            {
                spawnTimer -= spawnInterval;
                SpawnByPatternType(pattern, ref spiralAngle);
            }

            yield return null;
        }

        onFinished?.Invoke();
        activeRoutine = null;
    }

    private void SpawnByPatternType(AttackPatternData pattern, ref float spiralAngle)
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
            case PatternType.Spiral:
                SpawnSpiral(pattern, ref spiralAngle);
                break;
            case PatternType.RingPulse:
                SpawnRingPulse(pattern);
                break;
            case PatternType.AimBurst:
                SpawnAimBurst(pattern);
                break;
            case PatternType.SineRain:
                SpawnSineRain(pattern);
                break;
            default:
                SpawnRain(pattern);
                break;
        }
    }

    private void SpawnRain(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 spawnPos = new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            rect.yMax + outsideSpawnOffset);

        SpawnBullet(spawnPos, Vector2.down * pattern.bulletSpeed, pattern);
    }

    private void SpawnHoming(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 spawnPos = RandomPointOnBorder(rect);
        Vector2 target = playerHeartTransform != null ? (Vector2)playerHeartTransform.position : rect.center;
        Vector2 direction = (target - spawnPos).normalized;
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.down;
        }

        SpawnBullet(spawnPos, direction * pattern.bulletSpeed, pattern);
    }

    private void SpawnPointBurst(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 center = RandomPointInside(rect);
        int count = Mathf.Max(1, pattern.bulletsPerSpawn);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float radians = (step * i) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            SpawnBullet(center, direction * pattern.bulletSpeed, pattern);
        }
    }

    private void SpawnSideStream(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        bool fromLeft = UnityEngine.Random.value < 0.5f;
        float jitter = Mathf.Abs(pattern.extraA);
        float y = UnityEngine.Random.Range(rect.yMin, rect.yMax) + UnityEngine.Random.Range(-jitter, jitter);
        y = Mathf.Clamp(y, rect.yMin, rect.yMax);

        Vector2 spawnPos = fromLeft
            ? new Vector2(rect.xMin - outsideSpawnOffset, y)
            : new Vector2(rect.xMax + outsideSpawnOffset, y);

        Vector2 velocity = fromLeft
            ? Vector2.right * pattern.bulletSpeed
            : Vector2.left * pattern.bulletSpeed;

        SpawnBullet(spawnPos, velocity, pattern);
    }

    private void SpawnSpiral(AttackPatternData pattern, ref float spiralAngle)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 origin = pattern.extraC > 0.5f ? RandomPointInside(rect) : rect.center;
        float radians = spiralAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        SpawnBullet(origin, direction * pattern.bulletSpeed, pattern);
        spiralAngle += pattern.extraA;
    }

    private void SpawnRingPulse(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 origin = rect.center;
        int count = Mathf.Max(1, pattern.bulletsPerSpawn);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float radians = (step * i) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            SpawnBullet(origin, direction * pattern.bulletSpeed, pattern);
        }
    }

    private void SpawnAimBurst(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 origin = rect.center;
        Vector2 target = playerHeartTransform != null ? (Vector2)playerHeartTransform.position : origin + Vector2.down;
        Vector2 forward = (target - origin).normalized;
        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector2.down;
        }

        int count = Mathf.Max(1, pattern.bulletsPerSpawn);
        float coneAngle = Mathf.Max(0f, pattern.extraA);
        float startAngle = -coneAngle * 0.5f;
        float step = count > 1 ? coneAngle / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + (step * i);
            Vector2 direction = Rotate(forward, angle);
            SpawnBullet(origin, direction * pattern.bulletSpeed, pattern);
        }
    }

    private void SpawnSineRain(AttackPatternData pattern)
    {
        if (!TryGetArenaRect(out Rect rect)) return;

        Vector2 spawnPos = new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            rect.yMax + outsideSpawnOffset);

        Bullet bullet = SpawnBullet(spawnPos, Vector2.down * pattern.bulletSpeed, pattern);
        if (bullet == null)
        {
            return;
        }

        float frequency = pattern.extraA;
        float amplitude = pattern.extraB;
        float phase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        bullet.EnableLateralSine(frequency, amplitude, phase);
    }

    private Bullet SpawnBullet(Vector2 position, Vector2 velocity, AttackPatternData pattern)
    {
        Bullet bullet = pool.Get(position);
        if (bullet == null)
        {
            return null;
        }

        int damage = ResolveDamage(pattern);
        bullet.Init(position, velocity, bulletLifetime, pool, arenaBounds, pattern.bulletProfile, damage);
        return bullet;
    }

    private int ResolveDamage(AttackPatternData pattern)
    {
        if (pattern.bulletDamageOverride >= 0)
        {
            return pattern.bulletDamageOverride;
        }

        if (pattern.bulletProfile != null)
        {
            return pattern.bulletProfile.baseDamage;
        }

        return 1;
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

    private Vector2 RandomPointInside(Rect rect)
    {
        return new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            UnityEngine.Random.Range(rect.yMin, rect.yMax));
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

    private Vector2 Rotate(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            (vector.x * cos) - (vector.y * sin),
            (vector.x * sin) + (vector.y * cos));
    }
}
