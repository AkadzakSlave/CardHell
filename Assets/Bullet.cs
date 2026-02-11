using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;

    private Vector2 direction = Vector2.down;
    private float timer;
    private BulletPool ownerPool;
    private Rect arenaRect;
    private bool hasArenaRect;

    public void Launch(Vector2 startPosition, Vector2 moveDirection, float moveSpeed, float maxLifeTime, Rect? optionalArenaRect, BulletPool pool)
    {
        transform.position = startPosition;
        direction = moveDirection.sqrMagnitude > 0f ? moveDirection.normalized : Vector2.down;
        speed = moveSpeed;
        lifeTime = maxLifeTime;
        ownerPool = pool;
        timer = 0f;

        if (optionalArenaRect.HasValue)
        {
            arenaRect = optionalArenaRect.Value;
            hasArenaRect = true;
        }
        else
        {
            hasArenaRect = false;
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            Deactivate();
            return;
        }

        if (hasArenaRect)
        {
            Vector2 pos = transform.position;
            if (!arenaRect.Contains(pos))
            {
                Deactivate();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    private void TryDamagePlayer(GameObject hit)
    {
        PlayerHeartController player = hit.GetComponent<PlayerHeartController>();
        if (player == null)
        {
            player = hit.GetComponentInParent<PlayerHeartController>();
        }

        if (player != null)
        {
            player.TakeDamage(1);
            Deactivate();
        }
    }

    public void Deactivate()
    {
        if (ownerPool != null)
        {
            ownerPool.Return(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
