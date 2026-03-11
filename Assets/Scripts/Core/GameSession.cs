using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Progress")]
    public int floorIndex = 1;
    public int coins = 0;

    [Header("Player Stats")]
    public int maxHP = 20;
    public int hp = 20;

    [Header("Battle")]
    public string selectedEnemyId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (hp > maxHP)
        {
            hp = maxHP;
        }
    }

    public void SelectEnemy(string enemyId)
    {
        selectedEnemyId = enemyId;
    }

    public void RestoreHPToMax()
    {
        hp = maxHP;
    }

    public void AddCoins(int amount)
    {
        coins += Mathf.Max(0, amount);
    }

    public void SpendCoins(int amount)
    {
        coins = Mathf.Max(0, coins - Mathf.Max(0, amount));
    }
}

