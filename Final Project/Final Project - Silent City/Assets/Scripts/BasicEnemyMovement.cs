using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public Transform player;                  // Reference to the player's transform
    public float moveSpeed = 1.0f;            // Speed at which the enemy chases the player
    public float directionChangeDelay = 0.5f; // Delay before changing direction, in seconds

    private bool isMoving = true;             // Tracks if the enemy is allowed to move
    private float timeSinceLastChange;        // Timer to track delay
    private int currentDirection;             // Tracks current movement direction (-1 for left, 1 for right)
    private SpriteRenderer spriteRenderer;    // Reference to the SpriteRenderer component
    private Animator animator;                // Reference to the Animator component

    void Start()
    {
        // Initialize direction, SpriteRenderer, and Animator
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Set initial direction based on player’s position
        UpdateSpriteDirection();
    }

    void Update()
    {
        if (player != null)
        {
            // Check if the enemy should pause before changing direction
            int newDirection = player.position.x > transform.position.x ? 1 : -1;

            if (newDirection != currentDirection)
            {
                // Enemy needs to change direction, start delay timer
                isMoving = false;
                timeSinceLastChange += Time.deltaTime;

                // Once delay is reached, allow movement and update direction
                if (timeSinceLastChange >= directionChangeDelay)
                {
                    isMoving = true;
                    timeSinceLastChange = 0;
                    currentDirection = newDirection;

                    // Update sprite direction to face player
                    UpdateSpriteDirection();
                }
            }

            // Move only if allowed to move
            if (isMoving)
            {
                // Calculate target position only along the x-axis
                Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

                // Move the enemy
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Set Animator parameter to true to trigger walking animation
                animator.SetBool("IsMoving", true);
            }
            else
            {
                // Set Animator parameter to false to trigger idle animation
                animator.SetBool("IsMoving", false);
            }
        }
    }

    // Helper method to update sprite direction to always face the player
    private void UpdateSpriteDirection()
    {
        // Flip sprite to face the player based on their relative position
        spriteRenderer.flipX = player.position.x > transform.position.x;
    }
}
