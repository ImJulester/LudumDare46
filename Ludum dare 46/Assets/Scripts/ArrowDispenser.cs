using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDispenser : MonoBehaviour
{

    public float timeBeforeShooting = 3;
    public GameObject arrow;
    private float originalTime;

    // Start is called before the first frame update
    void Start()
    {
        originalTime = timeBeforeShooting;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeBeforeShooting <= 0)
        {
            timeBeforeShooting = originalTime;
            Instantiate(arrow, transform.position, arrow.transform.localRotation);
        }
        else
        {
            timeBeforeShooting -= Time.deltaTime;
        }
    }
}
