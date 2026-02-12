using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles heart movement and damage i-frames with blink feedback.
/// If PlayerHealth exists on the same object, this controller writes damage there.
/// </summary>
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

    public event Action<int, int> OnHealthChanged;

    private Rigidbody2D rb;
    private PlayerHealth playerHealth;
    private Vector2 moveInput;
    private bool isInvincible;
    private Coroutine invincibilityRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.SetMaxHP(maxHP);
            hp = playerHealth.hp;
            maxHP = playerHealth.maxHP;
            playerHealth.OnHealthChanged += HandleHealthChangedFromComponent;
        }
        else
        {
            hp = Mathf.Clamp(maxHP, 0, maxHP);
            NotifyHealthChanged();
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChangedFromComponent;
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
        if (amount <= 0 || isInvincible)
        {
            return;
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(amount);
            hp = playerHealth.hp;
            maxHP = playerHealth.maxHP;
        }
        else
        {
            hp = Mathf.Clamp(hp - amount, 0, maxHP);
            NotifyHealthChanged();
        }

        if (invincibilityRoutine != null)
        {
            StopCoroutine(invincibilityRoutine);
        }

        invincibilityRoutine = StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        isInvincible = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        invincibilityRoutine = null;
    }

    private void HandleHealthChangedFromComponent(int currentHp, int currentMaxHp)
    {
        hp = currentHp;
        maxHP = currentMaxHp;
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(hp, maxHP);
    }
}
