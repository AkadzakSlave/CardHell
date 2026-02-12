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
    public Button startButton;
    public Button actButton;
    public EnemyVisual enemyVisual;
    public AttackDirector attackDirector;

    [Header("Progress")]
    [Range(0, 100)] public int friendship = 0;

    [Header("Events")]
    public BattleStateChangedEvent OnStateChanged;
    public FriendshipChangedEvent OnFriendshipChanged;

    [SerializeField] private BattleState state = BattleState.Idle;

    private Coroutine telegraphCoroutine;

    public BattleState State => state;

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

        SetState(BattleState.Idle);
        SetActButtonInteractable(false);
        NotifyFriendshipChanged();
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

    private void OnStartButtonClicked()
    {
        if (state == BattleState.Idle)
        {
            BeginBattle();
        }
    }

    private void OnActButtonClicked()
    {
        if (state != BattleState.PlayerTurn)
        {
            return;
        }

        friendship = Mathf.Clamp(friendship + 30, 0, 100);
        NotifyFriendshipChanged();

        if (enemyVisual != null)
        {
            enemyVisual.ShowEmbarrassed(1.0f);
        }

        SetActButtonInteractable(false);
        SetState(BattleState.EnemyTelegraph);

        if (telegraphCoroutine != null)
        {
            StopCoroutine(telegraphCoroutine);
        }

        telegraphCoroutine = StartCoroutine(StartEnemyAttackAfterDelay(1.0f));
    }

    public void BeginBattle()
    {
        friendship = 0;
        NotifyFriendshipChanged();

        SetState(BattleState.PlayerTurn);
        SetActButtonInteractable(true);
    }

    private IEnumerator StartEnemyAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SetState(BattleState.EnemyTurn);

        if (attackDirector != null)
        {
            attackDirector.PlayRandomPattern(HandleEnemyAttackFinished);
        }
        else
        {
            Debug.LogWarning("BattleController: AttackDirector is not assigned.");
            HandleEnemyAttackFinished();
        }

        telegraphCoroutine = null;
    }

    private void HandleEnemyAttackFinished()
    {
        SetState(BattleState.PlayerTurn);
        SetActButtonInteractable(true);
    }

    private void SetState(BattleState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(state);
    }

    private void SetActButtonInteractable(bool interactable)
    {
        if (actButton != null)
        {
            actButton.interactable = interactable;
        }
    }

    private void NotifyFriendshipChanged()
    {
        OnFriendshipChanged?.Invoke(friendship);
    }
}
