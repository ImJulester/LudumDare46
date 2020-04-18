using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayermask;
    [Header("Movement, 100 = 1 unit/second")]
    public float initDashSpeed;
    public float walkSpeed;
    public float runSpeed;

    public float accelerationRate;
    public float decelerationRate;

    public float jumpPower;

    public float gravity;

    public float friction;

    public float turnAroundSpeed;

    private float velocityX;
    private float velocityY;

    private float finalVelocityX;
    private float finalVelocityY;

    private bool grounded;

    private BoxCollider2D collider;
    private Animator anim;

    private float previousRunningInput = 0;

    private enum PlayerState {idle,walking,initDash,running,jump,land,falling,dying};

    PlayerState state = PlayerState.idle;


    // Start is called before the first frame update
    void Start()
    {
        initDashSpeed = initDashSpeed / 100.0f;
        runSpeed = runSpeed / 100.0f;
        accelerationRate = accelerationRate / 100.0f;
        decelerationRate = decelerationRate / 100.0f;
        jumpPower = jumpPower / 100.0f;
        gravity = gravity / 100.0f;
        friction = friction / 100.0f;
        turnAroundSpeed = turnAroundSpeed / 100.0f;

        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("player state : " + state.ToString());
        /*
         //movement controller
         if (Input.GetJoystickNames().Length > 0)
         {
             //hard coded deadzone
             if (Mathf.Abs(Input.GetAxis("C_Horizontal")) > 0.35f)
             {
                 if (Input.GetAxis("C_Horizontal") > 0)
                 {
                     if(velocityX <= runSpeed)
                     {
                         Run(true);
                     }
                 }
                 else if (Input.GetAxis("C_Horizontal") < 0)
                 {
                     if (velocityX >= -runSpeed)
                     {
                         Run(false);
                     }
                 }

             }
         }*/


        //if input while idle 
        //initDash
        //if init dash animation reached end
        //check if still input 
        // run or walk 

        //if no input back to idle

        //movement key

        if(state == PlayerState.idle)
        {

            if (Input.GetAxis("Horizontal") > 0)
            {
                state = PlayerState.initDash;
                anim.SetBool("InitDash", true);
                anim.SetBool("Idle", false);
                //set anim state to init dash
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                state = PlayerState.initDash;
                anim.SetBool("InitDash", true);
                anim.SetBool("Idle", false);
            }
            else
            {
                deceleration();
            }
        }

        if(state == PlayerState.initDash)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                InitDash(true);
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                InitDash(false);
            }
            else
            {
                state = PlayerState.idle;
                anim.SetBool("Idle", true);
                anim.SetBool("InitDash", false);
            }
        }
        if (state == PlayerState.running)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (previousRunningInput >= 0)
                {
                    Run(true);
                }
                else
                {
                    state = PlayerState.initDash;
                    anim.SetBool("InitDash", true);
                    anim.SetBool("Running", false);
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (previousRunningInput <= 0)
                {
                    Run(false);
                }
                else
                {
                    state = PlayerState.initDash;
                    anim.SetBool("InitDash", true);
                    anim.SetBool("Running", false);
                }
            }
            else
            {
                state = PlayerState.idle;
                anim.SetBool("Idle", true);
                anim.SetBool("Running", false);
            }
            previousRunningInput = Input.GetAxis("Horizontal");
        }



        /*if (Mathf.Abs(velocityX) > 0)
        {
            
            if (/*Input.GetAxis("C_Horizontal") == 0 && nput.GetAxis("Horizontal") == 0)
            {
                deceleration();
            }
        }*/

        //GRAVITY
        if (!grounded)
        {
            Gravity();
        }
        else
        {
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("C_Jump") && grounded)
            {
                anim.SetTrigger("Jump");
                //Jump();
            }
        }

       // Debug.Log("velocity x " + velocityX);
        yAxisCollision();
        xAxisCollision();

        Move(finalVelocityX, finalVelocityY);

    }

    void InitDash(bool right)
    {

        if (right)
        {
            velocityX = Mathf.MoveTowards(velocityX, initDashSpeed, accelerationRate * Time.deltaTime);
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, -initDashSpeed, accelerationRate * Time.deltaTime);
        }
    }

    public void EndInitDash()
    {
        if (Mathf.Abs( Input.GetAxis("Horizontal")) > 0)
        {
            Debug.Log("<Color=red END DASH </Color>");
            state = PlayerState.running;
            //set animator event 
            anim.SetBool("Running", true);
            anim.SetBool("InitDash", false);
            anim.SetBool("Idle", false);
        }
        else
        {
            state = PlayerState.idle;
            anim.SetBool("Idle", true);
            anim.SetBool("InitDash", false);
            anim.SetBool("Running", false);
        }
    }

    /*void Walk(bool right)
    {
        if (right)
        {
            velocityX = Mathf.MoveTowards(velocityX, walkSpeed, accelerationRate * Time.deltaTime);
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, -walkSpeed, accelerationRate * Time.deltaTime);
        }
    }*/

    void Run(bool right)
    {
        if (right)
        {
            if (velocityX <= runSpeed)
            {
                 velocityX = Mathf.MoveTowards(velocityX, runSpeed, accelerationRate * Time.deltaTime);
            }

        }
        else
        {
            if (velocityX >= -runSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, -runSpeed, accelerationRate * Time.deltaTime);
            }
        }
    }

    void Gravity()
    {
        velocityY -= gravity * Time.deltaTime;
    }
    void deceleration()
    {
        velocityX = Mathf.MoveTowards(velocityX, 0, decelerationRate * Time.deltaTime);
    }

    public void Jump()
    {
        velocityY = jumpPower;
        grounded = false;
        state = PlayerState.jump;
        anim.SetBool("Running", false);
        anim.SetBool("Idle", false);
        anim.SetBool("InitDash", false);
    }
    bool yAxisCollision()
    {
        float collisionOffset = 0.01f;
        if (velocityX > 0)
        {
            RaycastHit2D hitInfo2Dtop;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dbot;
            float collidedDistance = float.MaxValue;
            bool collided = false;
            hitInfo2Dtop = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, transform.position.y + (collider.bounds.extents.y - collisionOffset)), Vector2.right,Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, transform.position.y), Vector2.right, Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dbot = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, transform.position.y - (collider.bounds.extents.y - collisionOffset)), Vector2.right, Mathf.Abs(velocityX), groundLayermask);
            if (hitInfo2Dtop.collider != null)
            {
                collided = true;
                if (collidedDistance > hitInfo2Dtop.distance)
                {
                    collidedDistance = hitInfo2Dtop.distance;
                }
            }
            if (hitInfo2Dmid.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dtop.distance)
                {
                    collidedDistance = hitInfo2Dmid.distance;
                }
            }

            if (hitInfo2Dbot.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dbot.distance)
                {
                    collidedDistance = hitInfo2Dbot.distance;
                }
            }

            if (collided)
            {
                finalVelocityX = collidedDistance;
                return true;
            }
        }
        else
        {
            RaycastHit2D hitInfo2Dtop;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dbot;
            float collidedDistance = float.MaxValue;
            bool collided = false;
            hitInfo2Dtop = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, transform.position.y + (collider.bounds.extents.y - collisionOffset)), Vector2.left, Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, transform.position.y), Vector2.left, Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dbot = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, transform.position.y - (collider.bounds.extents.y - collisionOffset)), Vector2.left, Mathf.Abs(velocityX), groundLayermask);
            if (hitInfo2Dtop.collider != null)
            {
                collided = true;
                if (collidedDistance > hitInfo2Dtop.distance)
                {
                    collidedDistance = hitInfo2Dtop.distance;
                }
            }
            if (hitInfo2Dmid.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dtop.distance)
                {
                    collidedDistance = hitInfo2Dmid.distance;
                }
            }

            if (hitInfo2Dbot.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dbot.distance)
                {
                    collidedDistance = hitInfo2Dbot.distance;
                }
            }

            if (collided)
            {
                finalVelocityX = -collidedDistance;
                return true;
            }
        }
        finalVelocityX = velocityX;
        return false;
    }
    bool xAxisCollision()
    {
        float collisionOffset = 0.01f;
        if (velocityY > 0)
        {
            RaycastHit2D hitInfo2Dleft;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dright;
            float collidedDistance = float.MaxValue;
            bool collided = false;
            hitInfo2Dleft = Physics2D.Raycast(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset) , transform.position.y + collider.bounds.extents.y), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + collider.bounds.extents.y), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dright = Physics2D.Raycast(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y + collider.bounds.extents.y), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            if (hitInfo2Dleft.collider != null)
            {
                collided = true;
                if (collidedDistance > hitInfo2Dleft.distance)
                {
                    collidedDistance = hitInfo2Dleft.distance;
                }
            }
            if (hitInfo2Dmid.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dleft.distance)
                {
                    collidedDistance = hitInfo2Dmid.distance;
                }
            }

            if (hitInfo2Dright.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dright.distance)
                {
                    collidedDistance = hitInfo2Dright.distance;
                }
            }

            if (collided)
            {
                finalVelocityY = collidedDistance;
                return true;
            }
        }
        else
        {
            RaycastHit2D hitInfo2Dleft;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dright;
            float collidedDistance = float.MaxValue;
            bool collided = false;
            hitInfo2Dleft = Physics2D.Raycast(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset), transform.position.y - collider.bounds.extents.y), Vector2.down, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - collider.bounds.extents.y), Vector2.down, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dright = Physics2D.Raycast(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y - collider.bounds.extents.y), Vector2.down, Mathf.Abs(velocityY), groundLayermask);
            if (hitInfo2Dleft.collider != null)
            {
                collided = true;
                if (collidedDistance > hitInfo2Dleft.distance)
                {
                    collidedDistance = hitInfo2Dleft.distance;
                }
            }
            if (hitInfo2Dmid.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dleft.distance)
                {
                    collidedDistance = hitInfo2Dmid.distance;
                }
            }

            if (hitInfo2Dright.collider != null)
            {
                collided = true;

                if (collidedDistance > hitInfo2Dright.distance)
                {
                    collidedDistance = hitInfo2Dright.distance;
                }
            }

            if (collided)
            {
                finalVelocityY = -collidedDistance;
                Grounded(true);
                return true;
            }
        }
        finalVelocityY = velocityY;
        Grounded(false);
        return false;

    }

    void Move(float x, float y)
    {
        transform.position = transform.position +  new Vector3(x, y,0);
    }

    void Grounded(bool isGrounded)
    {
        if (isGrounded)
        {
            if(grounded == false)
            {
                grounded = true;
                velocityY = 0;
                if(state == PlayerState.jump)
                {
                    anim.SetTrigger("Land");
                    if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
                    {
                        state = PlayerState.running;
                        anim.SetBool("Running", true);
                        anim.SetBool("InitDash", false);
                        anim.SetBool("Idle", false);
                    }
                    else
                    {
                        state = PlayerState.idle;
                        anim.SetBool("InitDash", false);
                        anim.SetBool("Running", false);
                        anim.SetBool("Idle", true);
                    }
                    
                }
            }

        }
        if (!isGrounded)
        {
            grounded = false;
        }

    }


}
