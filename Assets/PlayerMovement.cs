using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Если назначен, игрок будет ограничен границами этого Collider2D (используются world bounds).")]
    [SerializeField] private Collider2D movementBoundsCollider;

    [Tooltip("Фолбэк-границы, если Collider2D не назначен. Значения в мировых координатах.")]
    [SerializeField] private Rect movementBoundsRect = new Rect(-8f, -4.5f, 16f, 9f);

    [Header("Health")]
    [SerializeField] private int health = 20;

    private Rigidbody2D rb;
    private Vector2 input;

    public int CurrentHealth => health;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Старая Input System: оси Horizontal/Vertical работают для WASD и стрелок.
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = Vector2.ClampMagnitude(input, 1f);
    }

    private void FixedUpdate()
    {
        Vector2 targetPosition = rb.position + input * moveSpeed * Time.deltaTime;
        rb.MovePosition(ClampToBounds(targetPosition));
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        health = Mathf.Max(health - amount, 0);
        Debug.Log($"Player HP: {health}");
    }

    private Vector2 ClampToBounds(Vector2 position)
    {
        Bounds bounds;

        if (movementBoundsCollider != null)
        {
            bounds = movementBoundsCollider.bounds;
            position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
            position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
            return position;
        }

        float minX = movementBoundsRect.xMin;
        float maxX = movementBoundsRect.xMax;
        float minY = movementBoundsRect.yMin;
        float maxY = movementBoundsRect.yMax;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}
