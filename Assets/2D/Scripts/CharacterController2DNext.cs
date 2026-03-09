using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DefaultExecutionOrder(-1000)]
public class CharacterController2DNext : MonoBehaviour
{
	[SerializeField] float speed = 1;
	[SerializeField] float jumpHeight = 2;

	[SerializeField] ContactFilter2D groundFilter;
	[SerializeField] Rigidbody2D.SlideMovement slideMovement = new Rigidbody2D.SlideMovement();

	// sprite
	Animator animator;
	SpriteRenderer spriteRenderer;
	int facing = 1; // 1 = right, -1 = left

	// input
	Vector2 moveInput = Vector2.zero;
	public void SetMoveInput(Vector2 input) => moveInput = input;

	bool jumpInput = false;
	public void SetJumpDown() => jumpInput = true;

	Rigidbody2D rb;
	Vector2 force;
	bool isGrounded = false;
	bool hasDoubleJump = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		isGrounded = slideMovement.selectedCollider.IsTouching(groundFilter);
		if (isGrounded)
		{
			hasDoubleJump = true;
		}

		Vector2 direction = Vector3.zero;
		direction.x = moveInput.x;//Input.GetAxis("Horizontal");

		force = direction * speed;

		if (jumpInput && (isGrounded || hasDoubleJump))
		{
			rb.linearVelocityY = 0;
			rb.AddForce(Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight * rb.gravityScale), ForceMode2D.Impulse);

			if (!isGrounded && hasDoubleJump)
			{
				hasDoubleJump = false;
			}
		}

		if (moveInput.x != 0)
		{
			if (Mathf.Sign(moveInput.x) != facing)
			{
				FlipDirection();
			}
		}

		animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
		jumpInput = false;
	}

	void FixedUpdate()
	{
		//rb.linearVelocity = new Vector2(force.x, rb.linearVelocity.y);

		if (isGrounded)
		{
			var slideVelocity = new Vector2(force.x, rb.linearVelocity.y);
			rb.Slide(slideVelocity, Time.fixedDeltaTime, slideMovement);
		}
		else
		{
			rb.linearVelocityX = force.x;
		}
	}

	void FlipDirection()
	{
		facing *= -1;
		spriteRenderer.flipX = (facing == -1);
	}
}
