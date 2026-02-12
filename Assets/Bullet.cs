using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 velocity;
    public int damage;
    public float lifetime;

    private BulletPool pool;
    private ArenaBounds arenaBounds;
    private float aliveTime;
    private const float OutsideMargin = 0.25f;

    public void Init(
        Vector2 position,
        Vector2 velocity,
        int damage,
        float lifetime,
        BulletPool pool,
        ArenaBounds arenaBounds)
    {
        transform.position = position;
        this.velocity = velocity;
        this.damage = damage;
        this.lifetime = lifetime;
        this.pool = pool;
        this.arenaBounds = arenaBounds;
        aliveTime = 0f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
        aliveTime += Time.deltaTime;

        if (aliveTime >= lifetime)
        {
            ReturnToPool();
            return;
        }

        if (arenaBounds != null)
        {
            Rect rect = arenaBounds.GetWorldRect();
            Rect expanded = Rect.MinMaxRect(
                rect.xMin - OutsideMargin,
                rect.yMin - OutsideMargin,
                rect.xMax + OutsideMargin,
                rect.yMax + OutsideMargin);

            if (!expanded.Contains(transform.position))
            {
                ReturnToPool();
            }
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
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
