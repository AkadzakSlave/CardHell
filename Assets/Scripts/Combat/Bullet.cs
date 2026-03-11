using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const float OutsideMargin = 0.25f;

    private BulletPool pool;
    private ArenaBounds arenaBounds;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private TrailRenderer trailRenderer;

    private Vector2 basePosition;
    private Vector2 baseVelocity;
    private Vector2 lateralAxis;
    private float lateralFrequency;
    private float lateralAmplitude;
    private float lateralPhase;
    private bool useLateralSine;

    private float lifetime;
    private float aliveTime;
    private int damage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Init(
        Vector2 position,
        Vector2 velocity,
        float lifetime,
        BulletPool pool,
        ArenaBounds arenaBounds,
        BulletProfileData profile,
        int damage)
    {
        transform.position = position;
        basePosition = position;
        baseVelocity = velocity;
        this.lifetime = lifetime;
        this.pool = pool;
        this.arenaBounds = arenaBounds;
        this.damage = damage;
        aliveTime = 0f;

        ConfigureVisuals(profile);
        DisableLateralSine();

        gameObject.SetActive(true);
    }

    public void EnableLateralSine(float frequency, float amplitude, float phase)
    {
        useLateralSine = true;
        lateralFrequency = frequency;
        lateralAmplitude = amplitude;
        lateralPhase = phase;

        lateralAxis = new Vector2(-baseVelocity.y, baseVelocity.x);
        float magnitude = lateralAxis.magnitude;
        if (magnitude > 0.0001f)
        {
            lateralAxis /= magnitude;
        }
        else
        {
            lateralAxis = Vector2.right;
        }
    }

    public void DisableLateralSine()
    {
        useLateralSine = false;
        lateralFrequency = 0f;
        lateralAmplitude = 0f;
        lateralPhase = 0f;
        lateralAxis = Vector2.zero;
    }

    private void ConfigureVisuals(BulletProfileData profile)
    {
        if (profile != null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = profile.sprite;
                spriteRenderer.color = profile.tint;
            }

            transform.localScale = new Vector3(profile.visualScale.x, profile.visualScale.y, 1f);

            if (circleCollider != null)
            {
                circleCollider.radius = Mathf.Max(0.001f, profile.hitboxRadius);
            }

            if (trailRenderer != null)
            {
                trailRenderer.enabled = profile.useTrail;
                trailRenderer.Clear();
            }

            return;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        transform.localScale = Vector3.one;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
            trailRenderer.Clear();
        }
    }

    private void Update()
    {
        aliveTime += Time.deltaTime;

        Vector2 position = basePosition + (baseVelocity * aliveTime);
        if (useLateralSine)
        {
            float offset = Mathf.Sin((aliveTime * lateralFrequency) + lateralPhase) * lateralAmplitude;
            position += lateralAxis * offset;
        }

        transform.position = position;

        if (aliveTime >= lifetime)
        {
            ReturnToPool();
            return;
        }

        if (arenaBounds == null)
        {
            return;
        }

        Rect rect = arenaBounds.GetWorldRect();
        Rect expanded = Rect.MinMaxRect(
            rect.xMin - OutsideMargin,
            rect.yMin - OutsideMargin,
            rect.xMax + OutsideMargin,
            rect.yMax + OutsideMargin);

        if (!expanded.Contains(position))
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHeartController player = other.GetComponent<PlayerHeartController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerHeartController>();
        }

        if (player != null)
        {
            player.TakeDamage(damage);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Return(this);
            return;
        }

        gameObject.SetActive(false);
    }
}
