using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;
    [SerializeField] private int hp;

    public event Action<int, int> OnHealthChanged;

    public int MaxHP => maxHP;
    public int HP => hp;

    private void Awake()
    {
        hp = Mathf.Max(0, maxHP);
        OnHealthChanged?.Invoke(hp, maxHP);
    }

    public void SetMaxHP(int value)
    {
        maxHP = Mathf.Max(1, value);
        hp = Mathf.Clamp(hp, 0, maxHP);
        OnHealthChanged?.Invoke(hp, maxHP);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        hp = Mathf.Clamp(hp + amount, 0, maxHP);
        OnHealthChanged?.Invoke(hp, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        hp = Mathf.Clamp(hp - amount, 0, maxHP);
        OnHealthChanged?.Invoke(hp, maxHP);
    }
}
