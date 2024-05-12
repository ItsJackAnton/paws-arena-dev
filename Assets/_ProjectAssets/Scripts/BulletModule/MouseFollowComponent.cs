using System.Collections;
using UnityEngine;

public class MouseFollowComponent : BulletComponent
{
    [SerializeField] private float followTime = 10f;
    [SerializeField] private float rotationCorrectionSpeed = 10f;
    [SerializeField] private AudioSource followingSound;
    [SerializeField] private LayerMask obstacleLayer; 
    [SerializeField] private float speed = 1f;

    private float raycastDistance = 0.5f;

    private float jumpForce=5f;
    private bool hasHitGround = false;
    private Transform playerToFollow;
    private bool canJump = true;

    protected override void Start()
    {
        base.Start();
        playerToFollow = isMine ? PlayerManager.Instance.otherPlayerTransform : PlayerManager.Instance.myPlayer.transform;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.localRotation.eulerAngles.z) > 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, 0)), rotationCorrectionSpeed * Time.deltaTime);
        }
        
        // Check for obstacles in front
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, raycastDistance, obstacleLayer);
        if (hit.collider != null && canJump)
        {
            StartCoroutine(JumpCooldown());
            float horizontalSpeed = Mathf.Abs(rb.velocity.x);
            float adjustedJumpForce = jumpForce + (horizontalSpeed * 0.1f); // Adjust the multiplier as needed

            rb.AddForce(Vector2.up * adjustedJumpForce, ForceMode2D.Impulse);
        }
        
        if (!hasHitGround) return;

        float dir = Mathf.Sign(playerToFollow.position.x - transform.position.x);
        float force = dir * Time.deltaTime * speed;
        rb.AddForce(new Vector2(force, 0), ForceMode2D.Force);     
        
        if(dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(1);
        canJump = true;
    }

    protected override void HandleCollision(Vector2 hitPose)
    {
        Debug.Log("landed");
        StartCoroutine(FollowEnemy(followTime, hitPose));
    }


    private IEnumerator FollowEnemy(float seconds, Vector2 hitPose)
    {
        ApplySettings();
        hasHitGround = true;
        followingSound.Play();

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        yield return new WaitForSeconds(seconds);

        followingSound.Stop();

        base.HandleCollision(transform.position);
    }

    private void ApplySettings()
    {
        followingSound.volume = GameState.gameSettings.soundFXVolume * GameState.gameSettings.masterVolume;
    }
}
