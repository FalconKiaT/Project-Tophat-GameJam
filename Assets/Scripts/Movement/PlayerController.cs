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

    //Collision Variables
    public float moveSpeed = 0.9f;
    public float collisionOffset = 0.02f; //Offset is to avoid clipping into walls
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    //Physics objects
    Vector2 movementInput;
    Rigidbody2D rgdb;

    // Start is called before the first frame update
    void Start()
    {
        rgdb = GetComponent<Rigidbody2D>();
        //Vector to allow for movement in x or y if one or the other is experiencing a collision
        xCheck = new Vector2(0, 0);
        yCheck = new Vector2(0, 0);
    }

    // Physics based update
    // (fixed 50 per second to avoid everything going haywire if fps goes too high)
    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
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
                //found collision on y
                success = TryMove(yCheck);
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {

        //Create raycast for collisions
        tally = rgdb.Cast(
                direction, //X and Y values between -1 and 1 that represents direction to look for collisions
                movementFilter, //The settings who determine where a collision can occur, like layers
                castCollisions, //Storage List of collisions found after cast was finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset //Cast distance equal to the movement plus an offset
                );

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

    //Function is called when player attempts to move, st
    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
}
