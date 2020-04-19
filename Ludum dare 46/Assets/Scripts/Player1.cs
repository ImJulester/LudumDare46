using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player1 : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayermask;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;

    public ParticleSystem groundHitParticle;

    public float jumpForce;
    public float moveSpeed;

    private Animator anim;
    private bool jumped;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            

            if (Mathf.Abs(Input.GetAxis("C_Horizontal"))> 0.2f)
            {
                rb2d.AddForce(new Vector2(Mathf.Sign(Input.GetAxis("C_Horizontal")) * moveSpeed, 0));
                anim.SetBool("Running", true);
            }
        }

        if(rb2d.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 3.75f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 3.75f);
        }

        if (Input.GetAxis("Horizontal") != 0)
        {

            rb2d.AddForce(new Vector2(Mathf.Sign(Input.GetAxis("Horizontal")) * moveSpeed, 0));
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }



    }

    void Update()
    {
        if (IsGrounded())
        {
            if (jumped)
            {
                anim.SetTrigger("Landing");
                GameObject.Instantiate(groundHitParticle.gameObject, transform.position + new Vector3(0,-0.5f,-0.1f), groundHitParticle.gameObject.transform.rotation);
                jumped = false;
            }
        }

        if (Input.GetJoystickNames().Length > 0)
        {
            if (Input.GetButtonDown("C_Jump") && IsGrounded())
            {
                //rb2d.AddForce(Vector2.up * jumpForce);
                anim.SetTrigger("Jump");
            }
        }
         if (Input.GetButtonDown("Jump") && IsGrounded())
         {
            //rb2d.AddForce(Vector2.up * jumpForce);
            anim.SetTrigger("Jump");
        }
        
    }

    bool IsGrounded()
    {
        float extraHeight = 0.01f;
        bool result;
        result = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, groundLayermask);

        if (result)
        {
            Debug.Log("<color=green>collision </color>");
        }
        else
        {
            Debug.Log("<color=red>no collision </color>");
        }

        return result;
    }
    public void Jump()
    {
        rb2d.AddForce(Vector2.up * jumpForce); 
    }

    public void CheckJumped()
    {
        jumped = true;
    }
}
