using System.Collections;
using UnityEngine;

public class AICharacter : MonoBehaviour
{
    public enum State
    {
        Any,    // Matches any state
        Idle,
        Patrol,
        Chase,
        Attack,
        Death
    }

    [Header("References")]
    [SerializeField] public CharacterController2D characterController;
    [SerializeField] private Animator animator;

    [Header("AI Settings")]
    [SerializeField] private float idleTime = 2f; // Default idle time
    [SerializeField] private float chaseRange = 5f; // Distance to start chasing the target
    [SerializeField] private float attackRange = 1f; // Distance to start attacking the target

    private GameObject target = null; // The target the AI is interacting with
    private State currentState = State.Idle;

    private void Start()
    {
        // Cache the player reference at start
        target = GameObject.FindGameObjectWithTag("Player");
    }

    public State CurrentState => currentState;

    private void Awake()
    {
        if (characterController == null)
        {
            Debug.LogError("CharacterController2D is not assigned!");
        }

        // Start in Patrol state
        SetState(State.Patrol);
    }

    private void Update()
    {
        HandleState();
    }

    /// <summary>
    /// Handles the AI's behavior based on its current state.
    /// </summary>
    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                // Do nothing, waiting for idle timer
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                ChaseTarget();
                break;

            case State.Attack:
                AttackTarget();
                break;

            case State.Death:
                // Handle death logic (e.g., disable movement)
                break;
        }
    }

    /// <summary>
    /// Sets the AI's state and handles transitions.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public void SetState(State newState)
    {
        if (currentState == newState) return;

        Debug.Log($"AI transitioning from {currentState} to {newState}");
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                StartCoroutine(IdleRoutine(idleTime));
                break;

            case State.Patrol:
                characterController.OnMove(new Vector2(RandomDirection(), 0));
                break;

            case State.Chase:
                // Chase logic will be handled in Update
                break;

            case State.Attack:
                characterController.OnMove(Vector2.zero); // Stop moving to attack
                animator?.SetTrigger("Attack");
                break;

            case State.Death:
                characterController.OnMove(Vector2.zero); // Stop movement
                break;
        }
    }

    public void Idle(float time)
    {
        Debug.Log($"AI is idling for {time} seconds.");
        StartCoroutine(IdleRoutine(time));
    }

    private IEnumerator IdleRoutine(float time)
    {
        currentState = State.Idle; // Set the state to Idle
        characterController.OnMove(Vector2.zero); // Stop movement
        yield return new WaitForSeconds(time); // Wait for the specified time
        SetState(State.Patrol); // Transition back to Patrol after idling
    }

    /// <summary>
    /// Handles patrolling behavior.
    /// </summary>
    private void Patrol()
    {
        if (target != null && Vector2.Distance(transform.position, target.transform.position) <= chaseRange)
        {
            SetState(State.Chase);
            return;
        }

        if (Random.value < 0.01f) // Random chance to flip direction
        {
            FlipDirection();
        }
    }

    /// <summary>
    /// Handles chasing the target.
    /// </summary>
    private void ChaseTarget()
    {
        if (target == null)
        {
            SetState(State.Patrol); // Return to patrol if no target
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (distanceToTarget > chaseRange)
        {
            SetState(State.Patrol); // Stop chasing if target is out of range
        }
        else if (distanceToTarget <= attackRange)
        {
            SetState(State.Attack); // Start attacking if within range
        }
        else
        {
            // Move towards the target
            Vector2 direction = (target.transform.position - transform.position).normalized;
            characterController.OnMove(new Vector2(Mathf.Sign(direction.x), 0));
        }
    }

    /// <summary>
    /// Handles attacking the target.
    /// </summary>
    private void AttackTarget()
    {
        if (target == null)
        {
            SetState(State.Patrol);
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (distanceToTarget > chaseRange)
        {
            SetState(State.Patrol);
        }
        else if (distanceToTarget > attackRange)
        {
            SetState(State.Chase); // Player backed out of attack range, resume chasing
        }
    }

    /// <summary>
    /// Flips the AI's direction.
    /// </summary>
    public void FlipDirection()
    {
        characterController.OnMove(new Vector2(characterController.Facing * -1, 0));
    }

    /// <summary>
    /// Sets a random direction for the AI.
    /// </summary>
    private int RandomDirection()
    {
        return Random.value < 0.5f ? CharacterController2D.FACE_LEFT : CharacterController2D.FACE_RIGHT;
    }

    /// <summary>
    /// Sets the target for the AI to chase or attack.
    /// </summary>
    /// <param name="newTarget">The target GameObject.</param>
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        SetState(State.Chase);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Reserved for attack damage handling
    }

    public void SetDirection(int direction)
    {
        // Set the movement direction using the CharacterController2D
        characterController.OnMove(new Vector2(direction, 0));
    }

}

