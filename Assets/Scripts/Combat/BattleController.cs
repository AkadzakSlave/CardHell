using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public enum BattleState
    {
        Idle,
        PlayerTurn,
        EnemyTelegraph,
        EnemyTurn
    }

    [System.Serializable]
    public class BattleStateChangedEvent : UnityEvent<BattleState> { }

    [System.Serializable]
    public class FriendshipChangedEvent : UnityEvent<int> { }

    [Header("References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button actButton;
    [SerializeField] private EnemyVisual enemyVisual;
    [SerializeField] private EnemyPresenter enemyPresenter;
    [SerializeField] private AttackDirector attackDirector;

    [Header("Enemy")]
    public EnemyData currentEnemy;

    [Header("Stats")]
    [Range(0, 100)]
    public int friendship;

    [Header("Events")]
    public BattleStateChangedEvent OnStateChanged;
    public FriendshipChangedEvent OnFriendshipChanged;

    [SerializeField] private BattleState currentState = BattleState.Idle;
    private Coroutine telegraphRoutine;

    public BattleState CurrentState => currentState;

    private void Awake()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (actButton != null)
        {
            actButton.onClick.AddListener(OnActButtonClicked);
        }
    }

    private void Start()
    {
        SetState(BattleState.Idle);
        UpdateButtonsForState();
        NotifyFriendshipChanged();

        if (currentEnemy != null)
        {
            LoadEnemy(currentEnemy);
        }
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        if (actButton != null)
        {
            actButton.onClick.RemoveListener(OnActButtonClicked);
        }
    }

    public void LoadEnemy(EnemyData enemy)
    {
        currentEnemy = enemy;
        friendship = 0;
        NotifyFriendshipChanged();

        if (enemyPresenter != null)
        {
            enemyPresenter.ApplyEnemy(currentEnemy);
        }

        if (telegraphRoutine != null)
        {
            StopCoroutine(telegraphRoutine);
            telegraphRoutine = null;
        }

        if (attackDirector != null)
        {
            attackDirector.StopAttacks();
        }

        SetState(BattleState.Idle);
        UpdateButtonsForState();
    }

    public void OnStartButtonClicked()
    {
        if (currentState != BattleState.Idle)
        {
            return;
        }

        SetState(BattleState.PlayerTurn);
        UpdateButtonsForState();
    }

    public void OnActButtonClicked()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        ActOptionData selectedAct = SelectActOption();
        if (selectedAct != null)
        {
            friendship = Mathf.Clamp(friendship + selectedAct.friendshipDelta, 0, 100);
            if (!string.IsNullOrEmpty(selectedAct.resultText))
            {
                Debug.Log($"ACT: {selectedAct.displayName} -> {selectedAct.resultText}");
            }
        }

        NotifyFriendshipChanged();

        if (friendship >= 100)
        {
            HandleWin();
            return;
        }

        if (enemyVisual != null)
        {
            enemyVisual.ShowEmbarrassed(1f);
        }

        SetState(BattleState.EnemyTelegraph);
        UpdateButtonsForState();

        if (telegraphRoutine != null)
        {
            StopCoroutine(telegraphRoutine);
        }

        telegraphRoutine = StartCoroutine(EnemyTurnAfterTelegraph());
    }

    private IEnumerator EnemyTurnAfterTelegraph()
    {
        yield return new WaitForSeconds(1f);

        if (currentState != BattleState.EnemyTelegraph)
        {
            telegraphRoutine = null;
            yield break;
        }

        SetState(BattleState.EnemyTurn);
        UpdateButtonsForState();

        AttackPatternData pattern = SelectRandomPattern();
        if (attackDirector == null || pattern == null)
        {
            EndEnemyTurn();
            yield break;
        }

        bool finished = false;
        attackDirector.PlayPattern(pattern, () => finished = true);

        while (!finished)
        {
            yield return null;
        }

        EndEnemyTurn();
    }

    protected virtual ActOptionData SelectActOption()
    {
        if (currentEnemy == null || currentEnemy.actOptions == null)
        {
            return null;
        }

        for (int i = 0; i < currentEnemy.actOptions.Count; i++)
        {
            if (currentEnemy.actOptions[i] != null)
            {
                return currentEnemy.actOptions[i];
            }
        }

        return null;
    }

    private AttackPatternData SelectRandomPattern()
    {
        if (currentEnemy == null || currentEnemy.patterns == null || currentEnemy.patterns.Count == 0)
        {
            return null;
        }

        int start = Random.Range(0, currentEnemy.patterns.Count);
        for (int i = 0; i < currentEnemy.patterns.Count; i++)
        {
            AttackPatternData candidate = currentEnemy.patterns[(start + i) % currentEnemy.patterns.Count];
            if (candidate != null)
            {
                return candidate;
            }
        }

        return null;
    }

    private void EndEnemyTurn()
    {
        telegraphRoutine = null;

        if (friendship >= 100)
        {
            HandleWin();
            return;
        }

        SetState(BattleState.PlayerTurn);
        UpdateButtonsForState();
    }

    private void HandleWin()
    {
        Debug.Log("You Win! Friendship reached 100.");

        if (telegraphRoutine != null)
        {
            StopCoroutine(telegraphRoutine);
            telegraphRoutine = null;
        }

        if (attackDirector != null)
        {
            attackDirector.StopAttacks();
        }

        SetState(BattleState.Idle);
        UpdateButtonsForState();
    }

    private void SetState(BattleState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(currentState);
    }

    private void NotifyFriendshipChanged()
    {
        OnFriendshipChanged?.Invoke(friendship);
    }

    private void UpdateButtonsForState()
    {
        if (startButton != null)
        {
            startButton.interactable = currentState == BattleState.Idle && currentEnemy != null;
        }

        if (actButton != null)
        {
            actButton.interactable = currentState == BattleState.PlayerTurn;
        }
    }
}
