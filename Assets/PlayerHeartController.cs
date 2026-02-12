using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHeartController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public ArenaBounds arenaBounds;

    [Header("Health")]
    public int maxHp = 20;
    public int hp = 20;

    [Header("Damage")]
    public float invincibilitySeconds = 0.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float invincibleUntil;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    private void Update()
    {
        // old Input System axes (WASD + arrows by default)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }

    private void FixedUpdate()
    {
        Vector2 target = rb.position + moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(ClampToArena(target));
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || hp <= 0)
        {
            return;
        }

        if (Time.time < invincibleUntil)
        {
            return;
        }

        hp = Mathf.Max(0, hp - dmg);
        invincibleUntil = Time.time + invincibilitySeconds;

        Debug.Log($"PlayerHeart HP: {hp}/{maxHp}");
    }

    private Vector2 ClampToArena(Vector2 pos)
    {
        if (arenaBounds == null)
        {
            return pos;
        }

        Rect r = arenaBounds.GetWorldRect();
        pos.x = Mathf.Clamp(pos.x, r.xMin, r.xMax);
        pos.y = Mathf.Clamp(pos.y, r.yMin, r.yMax);
        return pos;
    }
}
