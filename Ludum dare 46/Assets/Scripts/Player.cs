using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayermask;
    [Header("Movement, 100 = 1 unit/second")]
    public float maxSpeed;

    public float accelerationRate;
    public float decellerationRate;

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

    private enum PlayerAnimations {idle,walking,running,jump,land};
    PlayerAnimations playerAnim = PlayerAnimations.idle;


    // Start is called before the first frame update
    void Start()
    {
        maxSpeed = maxSpeed / 100.0f;
        accelerationRate = accelerationRate / 100.0f;
        decellerationRate = decellerationRate / 100.0f;
        jumpPower = jumpPower / 100.0f;
        gravity = gravity / 100.0f;
        friction = friction / 100.0f;
        turnAroundSpeed = turnAroundSpeed / 100.0f;


        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //movement controller
        if (Input.GetJoystickNames().Length > 0)
        {
            if (Mathf.Abs(Input.GetAxis("C_Horizontal")) > 0.2f)
            {
                if (Input.GetAxis("C_Horizontal") > 0)
                {
                    if(velocityX <= maxSpeed)
                    {
                        velocityX = Mathf.MoveTowards(velocityX, maxSpeed, accelerationRate* Time.deltaTime);
                    }
                }
                else if (Input.GetAxis("C_Horizontal") < 0)
                {
                    if (velocityX >= -maxSpeed)
                    {
                        velocityX = Mathf.MoveTowards(velocityX,-maxSpeed, accelerationRate * Time.deltaTime);
                    }
                }
                
            }
        }

        //movement key
        Debug.Log(Input.GetAxis("Horizontal"));
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (velocityX <= maxSpeed)
                {
                    velocityX = Mathf.MoveTowards(velocityX, maxSpeed, accelerationRate * Time.deltaTime);
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (velocityX >= -maxSpeed)
                {
                    velocityX = Mathf.MoveTowards(velocityX, -maxSpeed, accelerationRate * Time.deltaTime);
                }
            }

        if (Mathf.Abs(velocityX) > 0.01)
        {
            if (Input.GetAxis("C_Horizontal") == 0 && Input.GetAxis("Horizontal") == 0)
            {
                velocityX = Mathf.MoveTowards(velocityX, 0, decellerationRate * Time.deltaTime);
            }
        }
        else
        {
            velocityX = 0;
        }


        /*if (!grounded)
        {
            velocityY -= gravity * Time.deltaTime;
        }
        else
        {
            if (Input.GetJoystickNames().Length > 0)
            {
                if (Input.GetButtonDown("C_Jump") && grounded)
                {
                    
                }
            }
            if (Input.GetButtonDown("Jump") && grounded)
            {
                velocityY = jumpPower;
                grounded = false;
            }
        }*/
        if (!grounded)
        {
            velocityY -= gravity * Time.deltaTime;
        }
        else
        {
            if (Input.GetButtonDown("Jump") && grounded)
            {
                velocityY = jumpPower;
                grounded = false;
            }
        }

        Debug.Log("final velocity x" + finalVelocityX + " final velocity y" + finalVelocityY);
        Debug.Log("horizontal collision? : " + yAxisCollision());
        Debug.Log("Vertical collision? : " + xAxisCollision());

        Move(finalVelocityX, finalVelocityY);


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
                Grounded();
                return true;
            }
        }
        finalVelocityY = velocityY;
        return false;
    }

    void Move(float x, float y)
    {
        Debug.Log(x + " y : " + y);
        transform.position = transform.position +  new Vector3(x, y,0);
    }

    void Grounded()
    {
        grounded = true;
    }
}
