using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    CharacterState characterState;
    [Range(0, 100f)] [SerializeField] public float moveSpeed = 50f;

    //public float moveSpeed;
    public float jumpForce = 8f;
    public Transform ceilingCheck;
    public Transform groundCheck;
    public LayerMask groundObjects;
    public float checkRadius;
    public int maxJumpCount;

    private Rigidbody2D rb;
    private bool facingRight = true;
    private float moveDirection;
    private bool isJumping = false;
    private bool isGrounded;
    private int jumpCount;
   


     
    // Awake is called after all objects are initialized. Called in a random order
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Will look for a component on this GameObject (what the script is attached to) of type Rigidbody2D.
    }
    

    private void Start()
    {
        jumpCount = maxJumpCount;
        characterState = GetComponent<CharacterState>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get Inputs
        ProcessInput();


        Animate();
        

    }


    //Better for handling Physics, can be called multiple times per update frame.
    private void FixedUpdate()
    {
        //// Check if grounded
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundObjects);
        //if (isGrounded)
        //{
        //    jumpCount = maxJumpCount;
        //}
        //Move();

        // Calculate the move factor at this step
        float moveFactor = characterState.horizontal * Time.deltaTime * moveSpeed;

        // Flipping sprite according to movement direction...
        if (moveFactor > 0 && !characterState.isFacingRight) flipSprite();
        else if (moveFactor < 0 && characterState.isFacingRight) flipSprite();

        // Let's move!
        characterState.rigidBody2D.velocity = new Vector2(moveFactor, characterState.rigidBody2D.velocity.y);



    }

    private void flipSprite()
    {
        characterState.isFacingRight = !characterState.isFacingRight;
        Vector3 transformScale = transform.localScale;
        transformScale.x *= -1;
        transform.localScale = transformScale;
    }

    private void Move()
    {
        // Move
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        if (isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpCount--;
        }
        isJumping = false;
    }

    private void Animate()
    {
        if (moveDirection > 0 && !facingRight)
        {
            FlipCharacter();
        }
        else if (moveDirection < 0 && facingRight)
        {
            FlipCharacter();
        }
    }

    private void ProcessInput()
    {
        moveDirection = Input.GetAxis("Horizontal"); // Scale of -1 -> 1.

        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            isJumping = true;
        }

    }

    private void FlipCharacter()
    {
        facingRight  = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

}
