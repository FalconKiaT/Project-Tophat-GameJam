using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircusBall : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 LastVelocity;
    Vector2 direction;
    Vector2 constantSpeed;

    //Random starting direction variables
    private string randomDir;
    public float ballSpeed = 1.5f;
    private string[] dirArray = { "upRight",
                                  "downRight",
                                  "downLeft", 
                                  "upLeft"};

    //Awake is called on object creation
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //Create a random direction to shoot the ball
        randomDir = dirArray[Random.Range(0, dirArray.Length)];
        rb.velocity = EstablishVector(randomDir);
        constantSpeed = rb.velocity;
    }

    //FixedUpdate used for physics
    private void FixedUpdate()
    {
        LastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collision behaviour of ball
        direction = Vector2.Reflect(LastVelocity.normalized, collision.contacts[0].normal);
        rb.velocity = direction * ballSpeed;
    }

    private Vector2 EstablishVector(string dir)
    {
        switch (dir)
        {
            case "upRight":
                return new Vector2(ballSpeed, ballSpeed);
            case "downRight":
                return new Vector2(ballSpeed, -1 * ballSpeed);
            case "downLeft":
                return new Vector2(-1 * ballSpeed, -1 * ballSpeed);
            case "upLeft":
                return new Vector2(-1 * ballSpeed, ballSpeed);
            default:
                //Abnormal set
                Debug.Log("WARNING! SOMETHING UNEXPECTED HAPPENED WHILE SETTING CIRCUS BALL DIRECTION");
                return Vector2.zero;
        }
    }
    
}
