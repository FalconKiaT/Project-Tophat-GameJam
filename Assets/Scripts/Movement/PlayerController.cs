using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Takes Input and handles the movement of the player inside the level
public class PlayerController : MonoBehaviour
{
    //Private variables for more efficient memory handling
    private bool success;
    private int tally;
    private Vector2 xCheck;
    private Vector2 yCheck;

    //Dashing Variables
    private bool isDashing;
    private bool canDash;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] TrailRenderer tr;
    [SerializeField] float trailTime;

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
    Rigidbody2D rgdb;

    // Start is called before the first frame update
    void Start()
    {
        isDashing = false;
        canDash = true;
        standardSpeed = 0.9f;
        moveSpeed = standardSpeed;
        dashCooldown = 1f;
        dashDuration = 0.1f;
        dashSpeed = 4f;
        collisionOffset = 0.02f;
        trailTime = 0.15f;

        rgdb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //Vector to allow for movement in x or y if one or the other is experiencing a collision
        xCheck = new Vector2(0, 0);
        yCheck = new Vector2(0, 0);
    }

    //Checking for other inputs
    void Update()
    {
        //Check if the player attempted to dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }


    // Physics based update
    // (fixed 50 per second to avoid everything going haywire if fps goes too high)
    private void FixedUpdate()
    {
        //Dont affect movement if dashing
        if (isDashing)
        {
            return;
        }
        else
        {
            //Draw trail
            
        }

        //Stop all movement if dead
        if (PlayerStats.player.dead)
        {
            //died, stop all movement
            animator.SetBool("died", true);
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
        tally = rgdb.Cast(
                direction, //X and Y values between -1 and 1 that represents direction to look for collisions
                movementFilter, //The settings that determine where a collision can occur, like layers
                castCollisions, //Storage List of collisions found after cast was finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset //Cast distance equal to the movement plus an offset
                );

        //Set animator to directon moving to
        animatorDirection(direction);

        //Tally stores how many collisions were found. if none, then move
        if (tally == 0)
        {
            //Move Player, No collisions found on x or y
            rgdb.MovePosition(rgdb.position + moveSpeed * Time.fixedDeltaTime * direction);
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

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        tr.emitting = true;

        rgdb.velocity = new Vector2(movementInput.x * dashSpeed, movementInput.y * dashSpeed);
        yield return new WaitForSeconds(dashDuration);
        //Reset velocity vector
        rgdb.velocity = new Vector2(0, 0);
        isDashing = false;
        tr.emitting = false;

        //Cooldown on dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void animatorDirection(Vector2 direction)
    {
        //switches to pick an animation direction
        //Vertical
        if (direction.y > 0) //Up
        {
            animator.SetBool("movedRight", false);
            animator.SetBool("movedLeft", false);
            animator.SetBool("movedUp", true);
            animator.SetBool("movedDown", false);
        }
        else if (direction.y < 0) //Down
        {
            animator.SetBool("movedRight", false);
            animator.SetBool("movedLeft", false);
            animator.SetBool("movedUp", false);
            animator.SetBool("movedDown", true);
        }
        else
        {
            //Y value is zero
        }
        
        //X side animation takes priority
        if (direction.x > 0) //Right
        {
            animator.SetBool("movedRight", true);
            animator.SetBool("movedLeft", false);
            animator.SetBool("movedUp", false);
            animator.SetBool("movedDown", false);
        }
        else if (direction.x < 0) //Left
        {
            animator.SetBool("movedRight", false);
            animator.SetBool("movedLeft", true);
            animator.SetBool("movedUp", false);
            animator.SetBool("movedDown", false);
        }
        else
        {
            //X is zero, nothing happened
        }

    }
    
}
