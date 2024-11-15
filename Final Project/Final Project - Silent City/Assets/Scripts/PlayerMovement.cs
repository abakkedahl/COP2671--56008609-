using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;       // Speed of the player
    public float jumpForce = 7f;       // Force applied when jumping
    private Rigidbody2D rb;            // Reference to the Rigidbody2D component
    public Animator animator;         // Reference to the Animator component
    private bool isGrounded = true;    // Check if the player is on the ground
    public float leftBoundary = -13.2f;
    public float rightBoundary = -1.1f;

    private AudioSource movementAudioSource;  // AudioSource for running sounds
    private AudioSource jumpAudioSource;      // AudioSource for jump sounds

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();                   // Get the Rigidbody2D component

        // Find and assign the AudioSource components
        movementAudioSource = GetComponents<AudioSource>()[0]; // First AudioSource, assumed to be for movement
        jumpAudioSource = GetComponents<AudioSource>()[1];     // Second AudioSource, assumed to be for jumping

        if (movementAudioSource == null || jumpAudioSource == null)
        {
            Debug.LogError("AudioSource components not found on this GameObject.");
        }
    }

    void Update()
    {
        // Get horizontal input (left/right)
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Move the player
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Keep the player within defined boundaries
        float clampedX = Mathf.Clamp(transform.position.x, leftBoundary, rightBoundary);
        transform.position = new Vector2(clampedX, transform.position.y);

        // Trigger jump animation if player presses W key and is grounded
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Apply jump force
            animator.SetBool("IsJumping", true);
            isGrounded = false; // Temporarily set to false until landing

            // Play the jump sound effect
            AudioClip jumpSound = Resources.Load<AudioClip>("SoundEffects/jump");
            if (jumpSound != null)
            {
                jumpAudioSource.PlayOneShot(jumpSound);  // Play jump sound on the second AudioSource
            }
            else
            {
                Debug.LogError("Jump sound not found in Resources folder.");
            }
        }

        // Flip the character's direction based on movement
        if (moveInput > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveInput < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        // Play running sound if the player is moving
        if (isGrounded && moveInput != 0)
        {
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();  // Play the running sound
            }
        }
        else
        {
            movementAudioSource.Stop();  // Stop the running sound when not moving
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is on the ground
            animator.SetBool("IsJumping", false);
        }
    }
}
