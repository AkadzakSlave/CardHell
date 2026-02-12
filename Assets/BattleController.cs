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

    [Header("Stats")]
    [Range(0, 100)]
    public int friendship = 0;

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
            actButton.interactable = false;
        }
    }

    private void Start()
    {
        SetState(BattleState.Idle);
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

    public void OnStartButtonClicked()
    {
        if (currentState == BattleState.Idle)
        {
            BeginBattle();
        }
    }

    public void BeginBattle()
    {
        friendship = 0;
        NotifyFriendshipChanged();

        if (actButton != null)
        {
            actButton.interactable = true;
        }

        SetState(BattleState.PlayerTurn);
    }

    public void OnActButtonClicked()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        friendship = Mathf.Clamp(friendship + 30, 0, 100);
        NotifyFriendshipChanged();

        if (enemyVisual != null)
        {
            enemyVisual.ShowEmbarrassed(1f);
        }

        if (actButton != null)
        {
            actButton.interactable = false;
        }

        SetState(BattleState.EnemyTelegraph);

        if (telegraphRoutine != null)
        {
            StopCoroutine(telegraphRoutine);
        }

        telegraphRoutine = StartCoroutine(EnemyTurnAfterTelegraph());
    }

    private IEnumerator EnemyTurnAfterTelegraph()
    {
        yield return new WaitForSeconds(1f);

        SetState(BattleState.EnemyTurn);

        if (attackDirector == null)
        {
            EndEnemyTurn();
            yield break;
        }

        bool finished = false;
        attackDirector.PlayRandomPattern(() => finished = true);

        while (!finished)
        {
            yield return null;
        }

        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        SetState(BattleState.PlayerTurn);

        if (actButton != null)
        {
            actButton.interactable = true;
        }

        telegraphRoutine = null;
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
}
