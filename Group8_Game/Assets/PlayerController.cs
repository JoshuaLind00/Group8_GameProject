using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D rb;
    public float MoveSpeed;
    public float WalkSpeed = 5f;
    public float RunSpeed = 8f;
    public float SpeedLimiter = 0.7f;
    float inputHorizontal;
    float inputVertical;
    public float stamina = 100;


    // Sets up rigid body at the start for future calls
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Regularly Checks for inputs to and sets moveSpeed
    void Update()
    {
        MoveSpeed = WalkSpeed;
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

    }

    void FixedUpdate()
    {
        //Limits player velocity to ensure that movement speed remains the constant if both directions are pressed
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            if (inputHorizontal != 0 && inputVertical != 0)
            {
                inputHorizontal *= SpeedLimiter;
                inputVertical *= SpeedLimiter;
            }
            rb.velocity = new Vector2(inputHorizontal * MoveSpeed, inputVertical * MoveSpeed);
        }
        else
        {
            rb.velocity = new Vector2(0f, 0f);
        }
    }

}
