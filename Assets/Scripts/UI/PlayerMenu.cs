using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMenu : MonoBehaviour
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

    //Animation Variablese
    Animator animator;

    //Collision Variables
    [SerializeField] float standardSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float collisionOffset; //Offset is to avoid clipping into walls
    public ContactFilter2D movementFilter;

    //Physics objects
    Vector2 movementInput;
    Vector2 direction;
    public Rigidbody2D rgbd;

    void Start()
    {
        canMove = false;
        standardSpeed = 0.9f;
        moveSpeed = standardSpeed;
        direction = new Vector2(-1, 1);

        rgbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //Vector to allow for movement in x or y if one or the other is experiencing a collision
        xCheck = new Vector2(0, 0);
        yCheck = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            direction = new Vector2(1, 1);
            rgbd.MovePosition(rgbd.position + moveSpeed * Time.fixedDeltaTime * direction);
            //Sends a bool to the animator to trigger movement animation
            animator.SetBool("isMoving", success);

            //Set animator to directon moving to
            animatorDirection(movementInput);
        }

        if (rgbd.position == new Vector2(3.66799f, -2.032012f))
        {
            endMovement();
        }
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

    public void startMovement()
    {
        canMove = true;
    }

    void endMovement()
    {
        canMove = false;
        SceneManager.LoadScene("Lounge");
    }
}
