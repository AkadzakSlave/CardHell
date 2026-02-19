using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugEnemySelectorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private List<EnemyData> enemiesForChapter = new List<EnemyData>();
    [SerializeField] private TMP_Dropdown tmpDropdown;
    [SerializeField] private Dropdown legacyDropdown;

    private int selectedIndex;

    private void Awake()
    {
        if (tmpDropdown != null)
        {
            tmpDropdown.onValueChanged.AddListener(OnTMPDropdownChanged);
        }

        if (legacyDropdown != null)
        {
            legacyDropdown.onValueChanged.AddListener(OnLegacyDropdownChanged);
        }
    }

    private void Start()
    {
        RefreshDropdownOptions();
        ApplySelection(0);
    }

    private void OnDestroy()
    {
        if (tmpDropdown != null)
        {
            tmpDropdown.onValueChanged.RemoveListener(OnTMPDropdownChanged);
        }

        if (legacyDropdown != null)
        {
            legacyDropdown.onValueChanged.RemoveListener(OnLegacyDropdownChanged);
        }
    }

    public void ReloadEnemy()
    {
        ApplySelection(selectedIndex);
    }

    private void OnTMPDropdownChanged(int index)
    {
        ApplySelection(index);
    }

    private void OnLegacyDropdownChanged(int index)
    {
        ApplySelection(index);
    }

    private void ApplySelection(int index)
    {
        selectedIndex = Mathf.Clamp(index, 0, Mathf.Max(0, enemiesForChapter.Count - 1));
        if (battleController == null || enemiesForChapter.Count == 0)
        {
            return;
        }

        EnemyData selected = enemiesForChapter[selectedIndex];
        battleController.LoadEnemy(selected);
    }

    private void RefreshDropdownOptions()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < enemiesForChapter.Count; i++)
        {
            EnemyData enemy = enemiesForChapter[i];
            names.Add(enemy != null && !string.IsNullOrEmpty(enemy.displayName)
                ? enemy.displayName
                : $"Enemy {i + 1}");
        }

        if (tmpDropdown != null)
        {
            tmpDropdown.ClearOptions();
            tmpDropdown.AddOptions(names);
            tmpDropdown.SetValueWithoutNotify(0);
        }

        if (legacyDropdown != null)
        {
            legacyDropdown.ClearOptions();
            legacyDropdown.AddOptions(names);
            legacyDropdown.SetValueWithoutNotify(0);
        }
    }
}
