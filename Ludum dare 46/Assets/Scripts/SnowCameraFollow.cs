using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowCameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform camera;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(camera != null)
        {
            transform.position = new Vector3(camera.position.x, transform.position.y, transform.position.z);
        }

        
    }
}
