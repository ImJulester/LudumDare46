using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : MonoBehaviour
{
    public float speed = 10;

    private bool destroyed;
    private float destroyTimer = 0.5f;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!destroyed)
        {
            transform.position += new Vector3((speed * transform.localScale.x) * Time.deltaTime, 0, 0);
        }
        else if (destroyTimer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            destroyTimer -= Time.deltaTime;
            sr.color = new Vector4(sr.color.r, sr.color.g, sr.color.b, destroyTimer * 4f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Ignore")
        {
            GetComponentInChildren<ParticleSystem>().Stop();
            GetComponent<CircleCollider2D>().enabled = false;
            destroyed = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Ignore")
        {
            GetComponentInChildren<ParticleSystem>().Stop();
            GetComponent<CircleCollider2D>().enabled = false;
            destroyed = true;
        }
    }
}
