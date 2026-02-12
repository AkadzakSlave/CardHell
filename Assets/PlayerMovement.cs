using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Tooltip("Playable area bounds. If null, fallbackRect is used.")]
    [SerializeField] private BoxCollider2D arenaCollider;

    [Tooltip("Fallback world rect when arenaCollider is not assigned.")]
    [SerializeField] private Rect fallbackRect = new Rect(-6f, -3f, 12f, 6f);

    [Header("Health")]
    public int maxHp = 30;
    public int hp = 30;

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    private void Update()
    {
        // old Input System axes (WASD + arrow keys by default)
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = Vector2.ClampMagnitude(input, 1f);
    }

    private void FixedUpdate()
    {
        Vector2 next = rb.position + input * moveSpeed * Time.deltaTime;
        rb.MovePosition(ClampToBounds(next));
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || hp <= 0)
        {
            return;
        }

        hp = Mathf.Max(0, hp - amount);
        Debug.Log($"Player HP: {hp}/{maxHp}");
    }

    private Vector2 ClampToBounds(Vector2 worldPos)
    {
        Rect rect = GetWorldRect();
        worldPos.x = Mathf.Clamp(worldPos.x, rect.xMin, rect.xMax);
        worldPos.y = Mathf.Clamp(worldPos.y, rect.yMin, rect.yMax);
        return worldPos;
    }

    private Rect GetWorldRect()
    {
        if (arenaCollider != null)
        {
            Bounds b = arenaCollider.bounds;
            return Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
        }

        return fallbackRect;
    }
}
