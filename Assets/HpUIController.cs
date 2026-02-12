using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates HP text and HP bar from PlayerHealth events.
/// Supports TMP text first, with optional legacy UI.Text fallback.
/// Supports Image Filled bar and RectTransform scale bar.
/// </summary>
public class HpUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Text (TMP preferred)")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Text hpTextLegacy;

    [Header("Bar (choose one)")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private RectTransform hpBarFill;

    private float initialScaleX = 1f;

    private void Awake()
    {
        if (hpBarFill != null)
        {
            initialScaleX = hpBarFill.localScale.x;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(playerHealth.hp, playerHealth.maxHP);
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
        UpdateText(hp, maxHP);
        UpdateBar(hp, maxHP);
    }

    private void UpdateText(int hp, int maxHP)
    {
        string value = $"HP {hp} / {maxHP}";

        if (hpText != null)
        {
            hpText.text = value;
        }

        if (hpTextLegacy != null)
        {
            hpTextLegacy.text = value;
        }
    }

    private void UpdateBar(int hp, int maxHP)
    {
        float ratio = maxHP > 0 ? hp / (float)maxHP : 0f;
        ratio = Mathf.Clamp01(ratio);

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = ratio;
        }

        if (hpBarFill != null)
        {
            Vector3 scale = hpBarFill.localScale;
            scale.x = initialScaleX * ratio;
            hpBarFill.localScale = scale;
        }
    }
}
