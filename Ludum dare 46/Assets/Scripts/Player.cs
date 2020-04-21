using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayermask;
    [Header("Movement, 100 = 1 unit/second")]
    public float initDashSpeed;
    public float runSpeed;
    public float crawlSpeed;
    public float airSpeed;



    public float accelerationRate;
    public float decelerationRate;

    public float jumpPower;

    public float gravity;


    public float velocityX;
    public float velocityY;

    private float finalVelocityX;
    private float finalVelocityY;

    private bool grounded;

    private bool stuckInCrouch = false;
    private bool died = false;

    private BoxCollider2D collider;
    private Animator anim;
    public SpriteRenderer spriteRenderer;

    private float previousRunningInput = 0;
    [Space(10)]
    [Header("flame values")]
    public Color fullFlameColor;
    public Color emptyFlameColor;

    public float maxFlame;
    public float startFlame;
    public float flameDecayRate;
    public float rainFlameDecayRate;
    public float snowDamage;
    private float flameValue;

    public Slider flameSlider;
    public Text flameValueText;
    public Image flameFill;
    public Color flameFillFull;
    public Color flameFillEmpty;
    public GameObject ParticleHitGround;
    public ParticleSystem fireParticle;
    public Light2D fireLight;
    
    public float minParticleLifetime;
    public float fullParticleLifetime;

    public GameObject deathPrefab;
    private AudioSource audioSource;

    public GameObject settingsCanvas;
    public GameObject finalCanvas;

    [Header("audio clips")]
    public AudioClip hitSnow;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip death;
    public AudioClip pickUp;
    private enum PlayerState { idle, walking, initDash, running, jump, land, falling, dying, crouch };

    public Vector2 spikeKnockbackPower;

    PlayerState state = PlayerState.idle;

    private GameObject activeSettings;

    

    // Start is called before the first frame update
    void Start()
    {
        flameSlider.maxValue = maxFlame;
        flameSlider.value = startFlame;
        fireParticle.startLifetime = fullParticleLifetime;

        initDashSpeed = initDashSpeed / 100.0f;
        runSpeed = runSpeed / 100.0f;
        crawlSpeed = crawlSpeed / 100.0f;
        airSpeed = airSpeed / 100.0f;
        accelerationRate = accelerationRate / 100.0f;
        decelerationRate = decelerationRate / 100.0f;
        jumpPower = jumpPower / 100.0f;
        gravity = gravity / 100.0f;
        flameValue = startFlame;
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && activeSettings == null)
        {
            activeSettings = Instantiate(settingsCanvas) as GameObject;
            activeSettings.GetComponent<Settings>().SetVolumes();
            Time.timeScale = 0;
        }
       
        if(state == PlayerState.dying)
        {
            return;
            //Play animation and destroy player?
            Debug.Log("DEAD");
        }
        ManageFlame();



        if (state == PlayerState.crouch)
        {
            float collisionOffset = 0.01f;
            RaycastHit2D hitInfo2Dleft;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dright;
            float collidedDistance = float.MaxValue;
            bool collided = false;

            hitInfo2Dleft = Physics2D.Raycast(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset), transform.position.y + 0.5f), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dright = Physics2D.Raycast(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y + 0.5f), Vector2.up, Mathf.Abs(velocityY), groundLayermask);

            if (hitInfo2Dleft.collider != null)
            {
                collided = true;
            }
            if (hitInfo2Dmid.collider != null)
            {
                collided = true;
            }

            if (hitInfo2Dright.collider != null)
            {
                collided = true;
            }

            stuckInCrouch = collided;


            if (Input.GetAxis("Vertical") >= 0)
            {
                if (!stuckInCrouch)
                {
                    stuckInCrouch = false;
                    collider.size = new Vector2(collider.size.x, 1);
                    collider.offset = new Vector2(0, 0);

                    if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
                    {
                        state = PlayerState.running;
                        anim.SetBool("Crouch", false);
                        anim.SetBool("Running", true);
                        //Maybe change this to init dash? 
                    }
                    else
                    {
                        state = PlayerState.idle;
                        anim.SetBool("Crouch", false);
                        anim.SetBool("Idle", true);
                    }
                }

            }
        }
        else if (Input.GetAxis("Vertical") < 0 && grounded)
        {
            state = PlayerState.crouch;
            collider.size = new Vector2(collider.size.x, collider.size.y / 2);
            collider.offset = new Vector2(0, -0.25f);
            anim.SetBool("Idle", false);
            anim.SetBool("InitDash", false);
            anim.SetBool("Running", false);
            anim.SetBool("Crouch", true);
        }


        if (grounded && !stuckInCrouch)
        {
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("C_Jump") && grounded)
            {
                anim.SetTrigger("Jump");
                //Jump();
            }
        }
    }
    void FixedUpdate()
    {
        
        //Debug.Log(state.ToString());
        if (state == PlayerState.dying)
        {
            return;
        }

        Move(finalVelocityX, finalVelocityY);

        if (state == PlayerState.crouch)
        {
            if (stuckInCrouch && velocityX < crawlSpeed)
            {
                if (previousRunningInput > 0)
                {
                    //Crawl(true);
                }
                else if (previousRunningInput < 0)
                {
                    //Crawl(false);
                }
            }
            else
            {
                deceleration();
            }
        }
       

        if (state == PlayerState.idle)
        {
            previousRunningInput = 0;
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

        if (state == PlayerState.initDash)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                previousRunningInput = Input.GetAxis("Horizontal");
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
                    previousRunningInput = Input.GetAxis("Horizontal");
                    Run(true);
                }
                else
                {
                    Debug.Log("<color=red>dashback </color>");
                    //Debug.Break();
                    state = PlayerState.initDash;
                    previousRunningInput = Input.GetAxis("Horizontal");
                    InitDash(false);
                    anim.SetBool("InitDash", true);
                    anim.SetBool("Running", false);
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (previousRunningInput <= 0)
                {
                    previousRunningInput = Input.GetAxis("Horizontal");
                    Run(false);
                }
                else
                {
                    Debug.Log("<color=red>dashback </color>");
                    //Debug.Break();
                    previousRunningInput = Input.GetAxis("Horizontal");
                    state = PlayerState.initDash;
                    InitDash(true);
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

        }

        if(state == PlayerState.jump)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                AirMovement(true);
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
               AirMovement(false);
            }
            else
            {
                deceleration();
            }
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

        
        xAxisCollision();
        yAxisCollision();
       

        

    }

    void PickupFlame(float amount)
    {
        flameValue += amount;
        if(flameValue > maxFlame)
        {
            flameValue = maxFlame;
        }
    }
    void ManageFlame()
    {
        flameValue -= flameDecayRate * Time.deltaTime;
        //Debug.Log("flamevalue : " + flameValue);
        spriteRenderer.color = Color.Lerp(emptyFlameColor, fullFlameColor, flameValue / maxFlame);

        flameSlider.value = flameValue;
        flameFill.color = Color.Lerp(flameFillEmpty, flameFillFull, flameValue / maxFlame);

        fireLight.intensity = Mathf.Lerp(0, 1.5f, flameValue/ maxFlame);

        fireParticle.startLifetime = Mathf.Lerp(minParticleLifetime,fullParticleLifetime , flameValue / maxFlame);

        flameValueText.text = Mathf.Ceil(flameValue) + "";

        if (flameValue <= 0)
        {
            Death();
            //DEATH
        }



    }

    void Death()
    {
        if (!died)
        {
            audioSource.PlayOneShot(death,1);
            spriteRenderer.enabled = false;
            state = PlayerState.dying;
            GameObject g = Instantiate(deathPrefab, transform.position, transform.rotation) as GameObject;
            SpriteRenderer[] sprites = g.GetComponentsInChildren<SpriteRenderer>();
            Rigidbody2D[] rb2ds = g.GetComponentsInChildren<Rigidbody2D>();
            int size = sprites.Length;

            for (int i = 0; i < size; i++)
            {
                sprites[i].color = spriteRenderer.color;
                rb2ds[i].AddForce(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 200);
            }
            StartCoroutine(deathWait());
            died = true;
        }
        
    }

    IEnumerator deathWait()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void InitDash(bool right)
    {

        if (right)
        {
            velocityX = Mathf.MoveTowards(velocityX, initDashSpeed, accelerationRate * Time.deltaTime);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, -initDashSpeed, accelerationRate * Time.deltaTime);
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void EndInitDash()
    {
        if (Mathf.Abs( Input.GetAxis("Horizontal")) > 0)
        {
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

    void AirMovement(bool right)
    {
        if (right)
        {
            if (velocityX <= airSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, airSpeed, accelerationRate * Time.deltaTime);
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
        else
        {
            if (velocityX >= -airSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, -airSpeed, accelerationRate * Time.deltaTime);
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
    void Run(bool right)
    {
        if (right)
        {
            if (velocityX <= runSpeed)
            {
                 velocityX = Mathf.MoveTowards(velocityX, runSpeed, accelerationRate * Time.deltaTime);
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
        else
        {
            if (velocityX >= -runSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, -runSpeed, accelerationRate * Time.deltaTime);
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
    void Crawl(bool right)
    {
        if (right)
        {
            if (velocityX <= crawlSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, crawlSpeed, accelerationRate * Time.deltaTime);
                transform.localScale = new Vector3(1, 1, 1);
            }

        }
        else
        {
            if (velocityX >= -crawlSpeed)
            {
                velocityX = Mathf.MoveTowards(velocityX, -crawlSpeed, accelerationRate * Time.deltaTime);
                transform.localScale = new Vector3(-1, 1, 1);
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

    public void SpawnLandingParticle()
    {
        GameObject.Instantiate(ParticleHitGround.gameObject, transform.position + new Vector3(0, -0.5f, -0.1f), ParticleHitGround.gameObject.transform.rotation);
    }

    public void Jump()
    {
        audioSource.PlayOneShot(jump);
        velocityY = jumpPower;
        grounded = false;
        state = PlayerState.jump;
        anim.SetBool("Running", false);
        anim.SetBool("Idle", false);
        anim.SetBool("InitDash", false);
        anim.SetBool("Crouch", false);
    }
    bool yAxisCollision()
    {
        float collisionOffset = 0.01f;
        if (velocityX >= 0)
        {
            RaycastHit2D hitInfo2Dtop;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dbot;
            float collidedDistance = float.MaxValue;
            bool collided = false;

#if UNITY_EDITOR
            Debug.DrawRay(new Vector2(transform.position.x + (collider.bounds.extents.x), collider.bounds.center.y + (collider.bounds.extents.y - collisionOffset)), Vector2.right, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y), Vector2.right, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y - collisionOffset)), Vector2.right, Color.red);

#endif

            hitInfo2Dtop = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y + (collider.bounds.extents.y - collisionOffset)), Vector2.right,Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y), Vector2.right, Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dbot = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y - collisionOffset)), Vector2.right, Mathf.Abs(velocityX), groundLayermask);
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

           /* if (!grounded)
            {
                RaycastHit2D hitInfo2DextraBot;
                float extraCheckOffset = 0.2f;

#if UNITY_EDITOR
                Debug.DrawRay(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y + extraCheckOffset)), Vector2.right, Color.red);
#endif
                hitInfo2DextraBot = Physics2D.Raycast(new Vector2(transform.position.x + collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y + extraCheckOffset)), Vector2.right, Mathf.Abs(velocityX), groundLayermask);

                if (hitInfo2DextraBot.collider != null)
                {
                    collided = true;

                    if (collidedDistance > hitInfo2DextraBot.distance)
                    {
                        collidedDistance = hitInfo2DextraBot.distance;
                    }
                }
            }*/

            if (collided)
            {
                velocityX = collidedDistance;
                finalVelocityX = collidedDistance;
                return true;
            }
        }
        else
        {
            RaycastHit2D hitInfo2Dtop;
            RaycastHit2D hitInfo2Dmid;
            RaycastHit2D hitInfo2Dbot;

#if UNITY_EDITOR
            Debug.DrawRay(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y + (collider.bounds.extents.y - collisionOffset)), Vector2.left, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y), Vector2.left, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y - collisionOffset)), Vector2.left, Color.red);

#endif

            float collidedDistance = float.MaxValue;
            bool collided = false;
            hitInfo2Dtop = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y + (collider.bounds.extents.y - collisionOffset)), Vector2.left, Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y), Vector2.left, Mathf.Abs(velocityX), groundLayermask);
            hitInfo2Dbot = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y - collisionOffset)), Vector2.left, Mathf.Abs(velocityX), groundLayermask);
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

           /* if (!grounded)
            {
                RaycastHit2D hitInfo2DextraBot;
                float extraCheckOffset = 0.2f;

#if UNITY_EDITOR
                Debug.DrawRay(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y + extraCheckOffset)), Vector2.left, Color.red);
#endif
              
                hitInfo2DextraBot = Physics2D.Raycast(new Vector2(transform.position.x - collider.bounds.extents.x, collider.bounds.center.y - (collider.bounds.extents.y + extraCheckOffset)), Vector2.left, Mathf.Abs(velocityX), groundLayermask);

                if (hitInfo2DextraBot.collider != null)
                {
                    collided = true;

                    if (collidedDistance > hitInfo2DextraBot.distance)
                    {
                        collidedDistance = hitInfo2DextraBot.distance;
                    }
                }
            }*/

            if (collided)
            {
                velocityX = -collidedDistance;
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

#if UNITY_EDITOR
            Debug.DrawRay(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset), transform.position.y + 0.5f), Vector2.up, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.up, Color.red);
            Debug.DrawRay(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y + 0.5f), Vector2.up, Color.red);

#endif

            hitInfo2Dleft = Physics2D.Raycast(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset) , transform.position.y + 0.5f), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dright = Physics2D.Raycast(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y + 0.5f), Vector2.up, Mathf.Abs(velocityY), groundLayermask);
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
                velocityY = collidedDistance;
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

#if UNITY_EDITOR
            Debug.DrawRay(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset), transform.position.y - 0.5f), Vector2.down , Color.red);
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.down , Color.red);
            Debug.DrawRay(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y - 0.5f), Vector2.down , Color.red);

#endif
            hitInfo2Dleft = Physics2D.Raycast(new Vector2(transform.position.x - (collider.bounds.extents.x - collisionOffset), transform.position.y - 0.5f), Vector2.down, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dmid = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.down, Mathf.Abs(velocityY), groundLayermask);
            hitInfo2Dright = Physics2D.Raycast(new Vector2(transform.position.x + (collider.bounds.extents.x - collisionOffset), transform.position.y - 0.5f), Vector2.down, Mathf.Abs(velocityY), groundLayermask);
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
                velocityY = -collidedDistance;
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
        float endX = x;
        float endY = y;
        transform.position = transform.position +  new Vector3(endX, endY,0);
        Physics2D.SyncTransforms();
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
                    audioSource.PlayOneShot(land);
                    if (Input.GetAxis("Vertical") < 0)
                    {
                        state = PlayerState.crouch;
                        anim.SetBool("Running", false) ;
                        anim.SetBool("InitDash", false);
                        anim.SetBool("Idle", false);
                        anim.SetBool("Crouch", true);
                    }
                    else if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
                    {
                        state = PlayerState.running;
                        anim.SetBool("Running", true);
                        anim.SetBool("InitDash", false);
                        anim.SetBool("Idle", false);
                        anim.SetBool("Crouch", false);
                    }
                    else
                    {
                        state = PlayerState.idle;
                        anim.SetBool("InitDash", false);
                        anim.SetBool("Running", false);
                        anim.SetBool("Idle", true);
                        anim.SetBool("Crouch", false);
                    }
                    
                }
            }

        }
        if (!isGrounded)
        {
            grounded = false;
            //state = PlayerState.jump;
        }

    }

    private void OnParticleCollision(GameObject other)
    {
        bool hit = false;
        ParticleSystem par = other.GetComponent<ParticleSystem>();
        ParticleSystem.Particle[] particle;
        particle = new ParticleSystem.Particle[par.main.maxParticles];
        int numParticlesAlive = par.GetParticles(particle);

        for (int i = 0; i < numParticlesAlive; i++)
        {
           
            if(Vector2.Distance(transform.position,particle[i].position) < 0.7f)
            {
                
                particle[i].remainingLifetime = -1;
                Vector3 velocity = particle[i].velocity;
                Debug.Log("velocity mangitude" + velocity.magnitude);
                if(!grounded)
                {
                    flameValue -= snowDamage;
                    hit = true;
                }
                
            }
        }
        par.SetParticles(particle);
        if (hit)
        {
            audioSource.PlayOneShot(hitSnow,0.3f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Collectible>() != null)
        {
            if (collision.tag == "Spike")
            {
                Debug.Log("HIT SPIKE");
                Vector2 dir = transform.position - collision.transform.position;
                dir.y += 1;
                dir.Normalize();
                dir *= spikeKnockbackPower;
                velocityX = dir.x;
                velocityY = dir.y;
                PickupFlame(collision.GetComponent<Collectible>().fireAmount);
                audioSource.PlayOneShot(hitSnow);
            }
            else
            {
                PickupFlame(collision.GetComponent<Collectible>().fireAmount);
                audioSource.PlayOneShot(pickUp, 1);
                Destroy(collision.gameObject);
            }

        }
        if(collision.gameObject.tag == "Portal")
        {
            SceneManager.LoadScene("FinalLevel 2");
        }
        if (collision.gameObject.tag == "FinalPortal")
        {
            state = PlayerState.dying;
            died = true;
            Instantiate(finalCanvas);
        }
    }
}
