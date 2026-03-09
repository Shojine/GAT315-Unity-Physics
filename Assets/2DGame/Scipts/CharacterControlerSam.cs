using UnityEngine;

public class CharacterControlerSam : MonoBehaviour
{
    [SerializeField] float speed = 1;

    // sprite
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    int facing = 1; // 1 = right, -1 = left

    // input
    Vector2 moveInput = Vector2.zero;
    

  

    Rigidbody2D rb;
    Vector2 force;

    public void OnMove(Vector2 v) => moveInput = v;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
       

        Vector2 direction = Vector3.zero;
        direction.x = moveInput.x;//Input.GetAxis("Horizontal");

        force = direction * speed;

        if (moveInput.x != 0)
        {
            if (Mathf.Sign(moveInput.x) != facing)
            {
                FlipDirection();
            }
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        
    }

    void FlipDirection()
    {
        facing *= -1;
        spriteRenderer.flipX = (facing == -1);
    }
}
