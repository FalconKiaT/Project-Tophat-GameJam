using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;

//Takes Input and handles the movement of the player inside the level
public class PlayerController : MonoBehaviour
{
    //Private variables for more efficient memory handling
    private bool success;
    private int tally;
    private Vector2 xCheck;
    private Vector2 yCheck;
    public bool canMove;
    public bool recievedDamage;
    public GameObject laserHurt;
    private AudioSource _laserDMG;
    [SerializeField] float dmgPushForce;
    [SerializeField] float dmgPushDuration;

    //Dashing Variables
    private bool canDash;
    public bool isDashing;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] TrailRenderer tr;
    PlaySoundOnTrigger _PlayerDash;

    //Animation Variablese
    Animator animator;

    //Collision Variables
    [SerializeField] float standardSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float collisionOffset; //Offset is to avoid clipping into walls
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    //Physics objects
    Vector2 movementInput;
    public Rigidbody2D rgbd;

    // Start is called before the first frame update
    void Start()
    {
        recievedDamage = false;
        dmgPushForce = 15f;
        dmgPushDuration = 0.2f;
        canMove = true;
        isDashing = false;
        canDash = true;
        standardSpeed = 0.9f;
        moveSpeed = standardSpeed;
        dashCooldown = 1f;
        dashDuration = 0.1f;
        dashSpeed = 4f;
        collisionOffset = 0.02f;
        _laserDMG = laserHurt.GetComponent<AudioSource>();

        rgbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _PlayerDash = GetComponent<PlaySoundOnTrigger>();
        //Vector to allow for movement in x or y if one or the other is experiencing a collision
        xCheck = new Vector2(0, 0);
        yCheck = new Vector2(0, 0);
    }

    //Checking for other inputs and damage recieved
    void Update()
    {
        //Check if the player attempted to dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && canMove)
        {
            StartCoroutine(Dash());
        }
    }

    // Physics based update
    // (fixed 50 per second to avoid everything going haywire if fps goes too high)
    private void FixedUpdate()
    {
        //Stop all movement if dead
        if (PlayerStats.player.dead)
        {
            //died, stop all movement
            animator.SetBool("died", true);
            rgbd.velocity = Vector2.zero;
            return;
        }

        //Dont affect movement if dashing or taking damage
        if (!canMove)
        {
            return;
        }

        if ( movementInput != Vector2.zero)
        {
            //General collision check, returns true if no collisions found on x or y
            success = TryMove(movementInput);

            //General collisions came back true, Now check only x
            xCheck.x = movementInput.x;
            if (!success)
            {
                success = TryMove(xCheck);
            }
            //Now check only y
            yCheck.y = movementInput.y;
            if (!success)
            {
                success = TryMove(yCheck);
            }
            //Sends a bool to the animator to trigger movement animation
            animator.SetBool("isMoving", success);

            //Set animator to directon moving to
            animatorDirection(movementInput);
        }
        else
        {
            //No movement, go idle
            animator.SetBool("isMoving", false);
        }
    }

    //Returns true if any collisions found, false for none
    private bool TryMove(Vector2 direction)
    {
        //Create raycast for collisions
        tally = rgbd.Cast(
                direction, //X and Y values between -1 and 1 that represents direction to look for collisions
                movementFilter, //The settings that determine where a collision can occur, like layers
                castCollisions, //Storage List of collisions found after cast was finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset //Cast distance equal to the movement plus an offset
                );

        

        //Tally stores how many collisions were found. if none, then move
        if (tally == 0)
        {
            //Move Player, No collisions found on x or y
            rgbd.MovePosition(rgbd.position + moveSpeed * Time.fixedDeltaTime * direction);
            return true;
        }

        //Found collision, dont move
        return false;
    }

    //Function is called when player attempts to move, used by Unity's input system
    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void animatorDirection(Vector2 direction)
    {
        //switches to pick an animation direction
        //Vertical
        if (direction.y > 0) //Up
        {
            animator.SetBool("movedUp", true);
            animator.SetBool("movedDown", false);
        }
        else if (direction.y < 0) //Down
        {
            animator.SetBool("movedUp", false);
            animator.SetBool("movedDown", true);
        }
        else
        {
            //Y is zero, either left or right
            animator.SetBool("movedUp", false);
            animator.SetBool("movedDown", false);
        }

        //X side animation takes priority
        if (direction.x > 0) //Right
        {
            animator.SetBool("movedRight", true);
            animator.SetBool("movedLeft", false);
        }
        else if (direction.x < 0) //Left
        {
            animator.SetBool("movedRight", false);
            animator.SetBool("movedLeft", true);
        }
        else
        {
            //X is zero, either up or down
            animator.SetBool("movedRight", false);
            animator.SetBool("movedLeft", false);
        }
    }

    private IEnumerator Dash()
    {
        LockMovement();
        canDash = false;
        isDashing = true;
        tr.emitting = true;
        // Ignore collisions with harmful objects while dashing
        // 7 = Harmful Objects / 6 = player
        Physics2D.IgnoreLayerCollision(6, 7, true);
        _PlayerDash.TriggerSound();

        rgbd.velocity = new Vector2(movementInput.x * dashSpeed, movementInput.y * dashSpeed);
        yield return new WaitForSeconds(dashDuration);
        //Reset velocity vector
        rgbd.velocity = new Vector2(0, 0);
        //Restore collision
        Physics2D.IgnoreLayerCollision(6, 7, false);
        isDashing = false;
        tr.emitting = false;
        UnlockMovement();

        //Cooldown on dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator TookDamage(UnityEngine.Transform enemyTransform)
    {
        if (!recievedDamage)
        {
            LockMovement();

            Vector2 pushDir;
            //Deal damage to player
            PlayerStats.player.TakeHit();
            //Move player away from object

            if (enemyTransform == null)
            {
                pushDir = RandomVector(-0.15f, 0.15f);
                //This is only used by lasers, so play their hit sound
                _laserDMG.Play();
            }
            else
            {
                pushDir = this.transform.position - enemyTransform.position;
            }
            rgbd.velocity = new Vector2(pushDir.x * dmgPushForce, pushDir.y * dmgPushForce);

            //Calculate damage source for animation
            if (0 < rgbd.velocity.x) //Hurt from the left
            {
                animator.SetBool("dmgFromLeft", true);
            }
            else
            {
                //Hurt from the right or default
                animator.SetBool("dmgFromRight", true);
            }

            //Trigger hurt animation
            animator.SetBool("hurt", true);

            //Trigger iFrames, stops this animation from executing again while iFrames are active
            StartCoroutine(iFramesTrigger());
            yield return new WaitForSeconds(dmgPushDuration);

            if (PlayerStats.player.dead)
            {
                animator.SetBool("died", true);
            }
            else
            {
                //Reset player back to his normal sprites
                animator.SetBool("hurt", false);
                animator.SetBool("dmgFromRight", false);
                animator.SetBool("dmgFromLeft", false);
            }

            //Reset player velocity
            rgbd.velocity = new Vector2(0, 0);
            UnlockMovement();
        }
    }

    private Vector2 RandomVector(float min, float max)
    {
        var x = Random.Range(min, max);
        var y = Random.Range(min, max);
        return new Vector2(x, y);
    }

    public IEnumerator iFramesTrigger()
    {
        recievedDamage = true;
        yield return new WaitForSeconds(PlayerStats.player.iFramesSecs);
        recievedDamage = false;
    }

    private void LockMovement()
    {
        canMove = false;
    }

    private void UnlockMovement()
    {
        canMove = true;
    }
    
}
