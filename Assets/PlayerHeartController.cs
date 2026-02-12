using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHeartController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private ArenaBounds arenaBounds;

    [Header("Health")]
    [SerializeField] private int maxHP = 20;
    [SerializeField] private float invincibilityDuration = 0.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isInvincible;

    public int CurrentHP { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        CurrentHP = maxHP;
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        if (arenaBounds != null)
        {
            Rect rect = arenaBounds.GetWorldRect();
            newPosition.x = Mathf.Clamp(newPosition.x, rect.xMin, rect.xMax);
            newPosition.y = Mathf.Clamp(newPosition.y, rect.yMin, rect.yMax);
        }

        rb.MovePosition(newPosition);
    }

    public void TakeDamage(int dmg)
    {
        if (dmg <= 0 || isInvincible || CurrentHP <= 0)
        {
            return;
        }

        CurrentHP = Mathf.Max(0, CurrentHP - dmg);
        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
