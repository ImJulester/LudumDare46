using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikeball : MonoBehaviour
{
    public bool reverse;
    public float rotateSpeed = 10;
    public float maxSpeed = 150;
    private float timer;
    private float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        

        if (reverse)
        {
            currentSpeed += rotateSpeed * Time.deltaTime;
            if (currentSpeed >= maxSpeed)
            {
                reverse = false;
            }
        }
        else
        {
            currentSpeed -= rotateSpeed * Time.deltaTime;
            if (currentSpeed <= maxSpeed * -1)
            {
                reverse = true;
                Debug.Log(currentSpeed);
            }
        }
    }
}
