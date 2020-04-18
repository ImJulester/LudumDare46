using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayermask;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;


    public float jumpForce;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            

            if (Mathf.Abs(Input.GetAxis("C_Horizontal"))> 0.2f)
            {
                rb2d.AddForce(new Vector2(Mathf.Sign(Input.GetAxis("C_Horizontal")) * moveSpeed, 0));
            }
        }
           

        if (Input.GetAxis("Horizontal") != 0)
        {

            rb2d.AddForce(new Vector2(Mathf.Sign(Input.GetAxis("Horizontal")) * moveSpeed, 0));
        }
        
       

    }

    void Update()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            if (Input.GetButtonDown("C_Jump") && IsGrounded())
            {
                rb2d.AddForce(Vector2.up * jumpForce);
            }
        }
         if (Input.GetButtonDown("Jump") && IsGrounded())
         {
            rb2d.AddForce(Vector2.up * jumpForce);
         }
        
    }


    bool IsGrounded()
    {
        float extraHeight = 0.02f;
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

}
