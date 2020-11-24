using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController controller;

    
    public bool canRun;
    public float runModifier = 2f;
    
    public bool canJump;
    public float jumpHeight = 3f;

    public float normalSpeed = 12f;
    public float gravity = -9.81f;

    public bool canCrouch;

    public float maxHeight;
    public float minHeight;


    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float speed;

    Vector3 velocity;
    bool isGrounded;


    // Update is called once per frame
    void Update()
    {
        //checks if the player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        if(Input.GetKey(KeyCode.LeftShift) && canRun)
        {
            speed = normalSpeed * runModifier;
        }
        else
        {
            speed = normalSpeed;
        }

        //controlling the crouch mechanic
        if (Input.GetKey(KeyCode.C))
        {
            //controller.height = 1f;
            if (controller.height > minHeight)
            {
                controller.height -= 0.05f;
            }

            speed /= 2;
        }
        else
        {
            if( controller.height < maxHeight)
            {
                controller.height += 0.05f;
            }
        }
       

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

///
/// Adjustments needed
///
/// 
/// 
/// Trouble shooting
///                 ---------   PLAYER WONT JUMP
///         make sure the gameobjects label is the same as the ground mask in the movement script
/// 
/// 
///
