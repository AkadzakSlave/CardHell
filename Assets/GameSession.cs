using UnityEngine;

public static class GameSession
{
    /// <summary>
    /// Заглушка сессии для battle-прототипа.
    /// Замените логику на загрузку сцены/передачу награды в вашем менеджере игры.
    /// </summary>
    public static void ReturnToOverworld(bool isWin, int coinsReward)
    {
        Debug.Log($"ReturnToOverworld called. Win: {isWin}, Coins: {coinsReward}");
    }
}
