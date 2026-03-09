using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

/// <summary>
/// A responsive 2D character controller that handles movement, jumping, and ground detection.
/// Features smooth acceleration, air control, coyote time, double jumping, and variable jump height.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 1f;              // Maximum movement speed
    [SerializeField] float acceleration = 10f;      // How quickly the character reaches top speed
    [SerializeField] float deceleration = 10f;      // How quickly the character stops
    [SerializeField] float airControl = 0.5f;       // Multiplier for acceleration while in air (0-1)

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;         // Maximum height of jump in units when button is held
    [SerializeField] float lowJumpMultiplier = 2f;  // Multiplier for gravity when jump button is released early (controls minimum jump height)
    [SerializeField] float fallMultiplier = 2.5f;   // Increases gravity when falling for better feel
    [SerializeField] float coyoteTime = 0.2f;       // Time in seconds character can jump after leaving ground
    [SerializeField] float doubleJumpTime = 0.5f;   // Time window in seconds to perform a double jump

    [Header("Ground Detection")]
    [SerializeField] float castDistance = 0.1f;     // How far to check for ground below the character
    [SerializeField] ContactFilter2D groundFilter;  // Layer and collision settings for ground detection

    [Header("Components")]
    [SerializeField] Animator animator;             // Reference to character's animator
    [SerializeField] SpriteRenderer spriteRenderer; // Reference to character's sprite renderer

    [Header("Events")]
    [SerializeField] UnityEvent onLand;              // Fired when the character lands on the ground

    [Header("Audio")]
    [SerializeField] private AudioClip walkingSound; // Footstep sound

    public Rigidbody2D rb;
    private AudioSource audioSource; // Reference to the AudioSource component
    private bool isWalking = false;// Reference to attached Rigidbody2D component

    public const int FACE_LEFT = -1; // Constant for left-facing direction
    public const int FACE_RIGHT = 1; // Constant for right-facing direction

    int facing = FACE_LEFT;                     // Facing direction: 1 = right, -1 = left

    public int Facing => facing;                               // Facing direction: 1 = right, -1 = left
    float currentSpeed = 0f;                       // Current horizontal speed after smoothing

    // Ground collision tracking
    RaycastHit2D[] raycastHits = new RaycastHit2D[5]; // Buffer for ground collision results
    int groundHits = 0;                              // Number of ground collisions detected
    bool isGrounded = false;                         // Whether character is currently on ground
    float coyoteTimer = 0f;                          // Timer for coyote time jump window
    float doubleJumpTimer = 0f;                      // Timer for double jump window

    // Jump state tracking
    bool jumpButtonReleased = true;                  // Whether jump button has been released since last press

    // Movement input
    Vector2 direction;                              // Current input direction (typically from -1 to 1)

    /// <summary>
    /// Sets the movement direction based on input.
    /// Called by input system when movement input changes.
    /// </summary>
    /// <param name="v">Vector2 containing horizontal and vertical input values</param>
    public void OnMove(Vector2 v) => direction = v;

    /// <summary>
    /// Initialize references. Called before Start.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();

        if (walkingSound != null)
        {
            audioSource.clip = walkingSound;
            audioSource.loop = true; // Loop the walking sound
        }
    }

    /// <summary>
    /// Handle non-physics updates. Called once per frame.
    /// </summary>
    void Update()
    {
        UpdateGroundCollision();
        UpdateFacing();
        UpdateAnimator();
        HandleWalkingSound(); // Check for walking sound
    }

    /// <summary>
    /// Handle physics-based movement. Called at fixed time intervals.
    /// </summary>
    void FixedUpdate()
    {
        // Calculate target speed based on input direction
        float targetSpeed = direction.x * speed;

        // Apply acceleration based on grounded state
        // Reduces control in the air by using the airControl multiplier
        float accelRate = isGrounded ? acceleration : acceleration * airControl;

        // Apply movement with smoothing
        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            // Accelerating towards target speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerating to stop
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        // Apply horizontal velocity
        rb.linearVelocityX = currentSpeed;

        // Apply variable jump height physics
        if (rb.linearVelocityY < 0)
        {
            // We're falling - apply increased gravity for snappier falls
            rb.linearVelocityY += Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocityY > 0 && jumpButtonReleased)
        {
            // We're rising but the jump button was released early
            // Apply extra gravity to cut the jump short (creates variable jump height)
            rb.linearVelocityY += Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Update character's facing direction based on movement input.
    /// </summary>
    void UpdateFacing()
    {
        // Only flip if we're moving and facing the opposite direction
        if (direction.x != 0 && Mathf.Sign(direction.x) != facing)
        {
            FlipDirection();
        }
    }

    /// <summary>
    /// Check for ground collision and update grounded state.
    /// Also handles landing logic and timer updates.
    /// </summary>
    void UpdateGroundCollision()
    {
        // Cast a ray downward to detect ground
        groundHits = rb.Cast(Vector2.down, groundFilter, raycastHits, castDistance);

        // Update grounded state
        bool wasGrounded = isGrounded;
        isGrounded = (groundHits > 0);

        // Handle landing (transitioning from in-air to grounded)
        if (isGrounded && !wasGrounded && rb.linearVelocityY < 0)
        {
            // Stop jump phase and vertical momentum when landing
            rb.linearVelocityY = 0;

            // Reset jump animation trigger to prevent retriggering
            animator?.ResetTrigger("Jump");

            onLand?.Invoke();
        }

        // Reset coyote timer when grounded
        if (isGrounded) coyoteTimer = coyoteTime;

        // Update timers
        coyoteTimer -= Time.deltaTime;
        doubleJumpTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Update animator parameters based on character state.
    /// </summary>
    void UpdateAnimator()
    {
        if (animator == null) return;


        animator.SetFloat("Speed", Mathf.Abs(direction.x));

    }

    /// <summary>
    /// Handle jump input. Called when jump button is pressed.
    /// </summary>
    public void OnJump()
    {
        if (coyoteTimer > 0)
        {
            // First jump - using coyote time for better feel
            jumpButtonReleased = false;
            doubleJumpTimer = doubleJumpTime;  // Enable double jump
            ExecuteJump();
        }
        else if (doubleJumpTimer > 0)
        {
            // Double jump - only possible during double jump time window
            jumpButtonReleased = false;
            doubleJumpTimer = 0;  // Consume the double jump
            ExecuteJump();
        }
    }

    /// <summary>
    /// Handle jump button release. Called when jump button is released.
    /// Used for variable jump height control.
    /// </summary>
    public void OnJumpRelease()
    {
        jumpButtonReleased = true;  // This flag is used in FixedUpdate to apply the lowJumpMultiplier
    }

    /// <summary>
    /// Perform the actual jump physics calculation and application.
    /// </summary>
    private void ExecuteJump()
    {
        // Calculate jump velocity using physics formula:
        // v = sqrt(2 * g * h) where g is gravity and h is desired height
        float jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight * rb.gravityScale);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);

        // Trigger jump animation if animator exists
        animator?.SetTrigger("Jump");
    }


    /// <summary>
    /// Trigger attack animation. Called by input system.
    /// </summary>

    public void OnAttack()
    {
        animator?.SetTrigger("Attack");
    }

    /// <summary>
    /// Trigger death animation. Called when character dies.
    /// </summary>
    public void OnDeath()
    {
        // Stop all movement and set velocity to zero
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;  // Disable physics interactions
        rb.simulated = false;   // Disable simulation
        animator?.SetTrigger("Death");
    }

    /// <summary>
    /// Trigger hit/damage animation. Called when character takes damage.
    /// </summary>
    public void OnHit()
    {
        animator?.SetTrigger("Hit");
    }

    /// <summary>
    /// Apply an impulse knockback force, overriding current velocity.
    /// </summary>
    public void Knockback(Vector2 force)
    {
        currentSpeed = force.x;
        rb.linearVelocity = force;
    }

    /// <summary>
    /// Flip the character's facing direction by updating the sprite.
    /// </summary>
    private void FlipDirection()
    {
        facing *= -1;  // Toggle between 1 and -1
        if (spriteRenderer != null)
            spriteRenderer.flipX = facing == FACE_LEFT;  // Flip sprite when facing left (sprite default is right)
    }

    /// <summary>
    /// Visualize ground detection in the editor. Only visible in Scene view when selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw rays showing ground contact points and normals
        if (groundHits > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < groundHits; i++)
            {
                Gizmos.DrawRay(raycastHits[i].point, raycastHits[i].normal);
            }
        }
    }


    // Key inventory to track collected keys
    private HashSet<string> keys = new HashSet<string>();

    /// <summary>
    /// Adds a key to the player's inventory.
    /// </summary>
    /// <param name="keyID">The unique ID of the key.</param>
    public void AddKey(string keyID)
    {
        InventoryManager.Instance.AddKey(keyID); // Add the key to the InventoryManager
    }

    public bool HasKey(string keyID)
    {
        return InventoryManager.Instance.HasKey(keyID); // Check the InventoryManager for the key
    }
    private void HandleWalkingSound()
    {
        // Check if the player is moving on the ground
        bool isMoving = Mathf.Abs(direction.x) > 0.1f && isGrounded;

        if (isMoving && !isWalking)
        {
            // Start playing the walking sound
            isWalking = true;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (!isMoving && isWalking)
        {
            // Stop playing the walking sound
            isWalking = false;
            audioSource.Stop();
        }
    }
}