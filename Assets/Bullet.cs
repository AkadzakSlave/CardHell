using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 velocity;
    public int damage = 1;
    public float lifetime = 3f;

    [HideInInspector] public BulletPool pool;
    [HideInInspector] public ArenaBounds arenaBounds;

    [SerializeField] private float outOfBoundsMargin = 0.25f;

    private float elapsed;
    private bool initialized;

    public void Init(Vector2 position, Vector2 newVelocity, int newDamage, float newLifetime, BulletPool newPool, ArenaBounds newArenaBounds)
    {
        transform.position = position;
        velocity = newVelocity;
        damage = newDamage;
        lifetime = newLifetime;
        pool = newPool;
        arenaBounds = newArenaBounds;

        elapsed = 0f;
        initialized = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        transform.position += (Vector3)(velocity * Time.deltaTime);
        elapsed += Time.deltaTime;

        if (elapsed > lifetime)
        {
            ReturnToPool();
            return;
        }

        if (arenaBounds != null)
        {
            Rect rect = arenaBounds.GetWorldRect();
            rect.xMin -= outOfBoundsMargin;
            rect.xMax += outOfBoundsMargin;
            rect.yMin -= outOfBoundsMargin;
            rect.yMax += outOfBoundsMargin;

            if (!rect.Contains(transform.position))
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

    public void ReturnToPool()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        initialized = false;

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
