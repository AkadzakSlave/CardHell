using UnityEngine;
using UnityEngine.UI;

public class HpUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Text hpText;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private RectTransform hpBarFill;

    private Vector3 initialBarScale = Vector3.one;

    private void Awake()
    {
        if (hpBarFill != null)
        {
            initialBarScale = hpBarFill.localScale;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(playerHealth.HP, playerHealth.MaxHP);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int hp, int maxHP)
    {
        string label = $"HP {hp} / {maxHP}";

        if (hpText != null)
        {
            hpText.text = label;
        }

        float ratio = maxHP > 0 ? hp / (float)maxHP : 0f;

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = ratio;
        }

        if (hpBarFill != null)
        {
            Vector3 scale = initialBarScale;
            scale.x *= ratio;
            hpBarFill.localScale = scale;
        }
    }
}
