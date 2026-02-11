using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHeartController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Tooltip("Границы арены через BoxCollider2D (рекомендуется).")]
    [SerializeField] private BoxCollider2D arenaBoundsCollider;

    [Tooltip("Фолбэк: Transform арены (центр). Используется вместе с Arena Size, если BoxCollider2D не задан.")]
    [SerializeField] private Transform arenaTransform;
    [SerializeField] private Vector2 arenaSize = new Vector2(12f, 7f);

    [Header("Health")]
    public int maxHP = 30;
    public int hp;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        hp = Mathf.Clamp(hp <= 0 ? maxHP : hp, 0, maxHP);
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }

    private void FixedUpdate()
    {
        Vector2 target = rb.position + moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(ClampToArena(target));
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || hp <= 0)
        {
            return;
        }

        hp = Mathf.Max(0, hp - amount);
        Debug.Log($"Player HP: {hp}/{maxHP}");

        if (hp <= 0)
        {
            BattleController battle = FindFirstObjectByType<BattleController>();
            if (battle != null)
            {
                battle.OnPlayerDied();
            }
            else
            {
                Debug.LogWarning("BattleController not found. Cannot call OnPlayerDied().");
            }
        }
    }

    public Rect GetArenaRect()
    {
        if (arenaBoundsCollider != null)
        {
            Bounds b = arenaBoundsCollider.bounds;
            return Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
        }

        if (arenaTransform != null)
        {
            Vector2 center = arenaTransform.position;
            Vector2 half = arenaSize * 0.5f;
            return Rect.MinMaxRect(center.x - half.x, center.y - half.y, center.x + half.x, center.y + half.y);
        }

        Vector2 fallbackCenter = transform.position;
        Vector2 fallbackHalf = arenaSize * 0.5f;
        return Rect.MinMaxRect(
            fallbackCenter.x - fallbackHalf.x,
            fallbackCenter.y - fallbackHalf.y,
            fallbackCenter.x + fallbackHalf.x,
            fallbackCenter.y + fallbackHalf.y);
    }

    private Vector2 ClampToArena(Vector2 position)
    {
        Rect rect = GetArenaRect();
        position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
        position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
        return position;
    }
}
