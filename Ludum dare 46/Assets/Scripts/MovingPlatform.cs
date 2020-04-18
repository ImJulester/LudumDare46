using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool movingDown;
    public float speed = 5;

    public GameObject upBorder;
    public GameObject downBorder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (movingDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, downBorder.transform.position, speed * Time.deltaTime);
            if(transform.position.x == downBorder.transform.position.x && transform.position.y == downBorder.transform.position.y)
            {
                movingDown = false;
            }
        }
        else
        {
            transform.position =  Vector3.MoveTowards(transform.position, upBorder.transform.position, speed * Time.deltaTime);
            if (transform.position.x == upBorder.transform.position.x && transform.position.y == upBorder.transform.position.y)
            {
                movingDown = true;
            }
        }
    }
}
