using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] runSprites;
    [SerializeField] Sprite climbSprite;
    private int spriteIndex;

    private  Rigidbody2D rigidbody;
    private  Collider2D collider;

    private Collider2D[] overlaps = new Collider2D[4];
    private Vector2 direction;

    private bool isOnGrounded;
    private bool isClimbing;

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float jumpStrength = 4f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f/12f, 1f/12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        CheckCollision();
        SetDirection();
    }

    private void CheckCollision()
    {
        isOnGrounded = false;
        isClimbing = false;

        Vector3 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, overlaps);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = overlaps[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                // Only set as grounded if the platform is below the player
                isOnGrounded = hit.transform.position.y < (transform.position.y - 0.5f);

                // Turn off collision on platforms the player is not grounded to
                Physics2D.IgnoreCollision(overlaps[i], collider, !isOnGrounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                isClimbing = true;
            }
        }
    }

    private void SetDirection()
    {
        if (isClimbing)
        {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;
        }
        else if (isOnGrounded && Input.GetKeyDown(KeyCode.Space))
        {
           direction = Vector2.up * jumpStrength;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxis("Horizontal") * moveSpeed;

        // Prevent gravity from building up infinitely
        if (isOnGrounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        RotatePlayer();
    }

    private void RotatePlayer()
    {
        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (isClimbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length) {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Dead");
            enabled = false;
            FindObjectOfType<GameManager>().LevelFailed();
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Win");
         //   enabled = false;
            FindObjectOfType<GameManager>().LevelComplete();
        }
            Debug.Log("trigger");
    }
}
