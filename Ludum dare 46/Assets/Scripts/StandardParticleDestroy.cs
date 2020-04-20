using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardParticleDestroy : MonoBehaviour
{
    public float timeBeforeDestroy = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeBeforeDestroy <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            timeBeforeDestroy -= Time.deltaTime;
        }
    }
}
