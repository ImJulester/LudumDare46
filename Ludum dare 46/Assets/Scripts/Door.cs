using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetTrigger("Open");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("je;d");
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<Animator>().SetTrigger("Open");
        }
    }
}
