using System;
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
    [SerializeField] private int hp;
    [SerializeField] private float invincibleDuration = 0.5f;
    [SerializeField] private float blinkInterval = 0.08f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerHealth playerHealth;

    public event Action<int, int> OnHealthChanged;

    public int HP => playerHealth != null ? playerHealth.HP : hp;
    public int MaxHP => playerHealth != null ? playerHealth.MaxHP : maxHP;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isInvincible;
    private Coroutine invincibilityCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.SetMaxHP(maxHP);
            playerHealth.OnHealthChanged += HandlePlayerHealthChanged;
            HandlePlayerHealthChanged(playerHealth.HP, playerHealth.MaxHP);
        }
        else
        {
            hp = Mathf.Max(0, maxHP);
            OnHealthChanged?.Invoke(hp, maxHP);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandlePlayerHealthChanged;
        }
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

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || isInvincible || HP <= 0)
        {
            return;
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(amount);
        }
        else
        {
            hp = Mathf.Clamp(hp - amount, 0, maxHP);
            OnHealthChanged?.Invoke(hp, maxHP);
        }

        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine());
    }

    private void HandlePlayerHealthChanged(int currentHp, int currentMaxHP)
    {
        hp = currentHp;
        maxHP = currentMaxHP;
        OnHealthChanged?.Invoke(currentHp, currentMaxHP);
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibleDuration)
        {
            elapsed += blinkInterval;
            visible = !visible;

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
            }

            yield return new WaitForSeconds(blinkInterval);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        isInvincible = false;
        invincibilityCoroutine = null;
    }
}
