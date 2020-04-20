using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDispenser : MonoBehaviour
{

    public float timeBeforeShooting = 3;
    public GameObject arrow;
    public bool reverse;
    private float originalTime;
    private GameObject shotArrow;

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
            shotArrow = Instantiate(arrow, transform.position, arrow.transform.localRotation);
            if (reverse)
            {
                shotArrow.transform.localScale = new Vector3(shotArrow.transform.localScale.x * -1, shotArrow.transform.localScale.y, shotArrow.transform.localScale.z);
            }
        }
        else
        {
            timeBeforeShooting -= Time.deltaTime;
        }
    }
}
