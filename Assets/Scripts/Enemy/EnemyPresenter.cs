using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPresenter : MonoBehaviour
{
    [SerializeField] private Image enemyCardImage;
    [SerializeField] private TMP_Text enemyNameText;

    public void ApplyEnemy(EnemyData enemy)
    {
        if (enemyCardImage != null)
        {
            enemyCardImage.sprite = enemy != null ? enemy.enemySprite : null;
            enemyCardImage.enabled = enemyCardImage.sprite != null;
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = enemy != null ? enemy.displayName : string.Empty;
        }
    }
}
