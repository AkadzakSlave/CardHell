using System;
using UnityEngine;

/// <summary>
/// Centralized player HP data source for battle logic and UI.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Min(1)]
    public int maxHP = 20;
    public int hp;

    /// <summary>
    /// (currentHp, maxHp)
    /// </summary>
    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        hp = Mathf.Clamp(maxHP, 0, maxHP);
        NotifyHealthChanged();
    }

    public void SetMaxHP(int value)
    {
        maxHP = Mathf.Max(1, value);
        hp = Mathf.Clamp(hp, 0, maxHP);
        NotifyHealthChanged();
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        hp = Mathf.Clamp(hp + amount, 0, maxHP);
        NotifyHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        hp = Mathf.Clamp(hp - amount, 0, maxHP);
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(hp, maxHP);
    }
}
