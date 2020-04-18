using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Transform player;

    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.zero;

    public Vector2 Boundsmin;
    public Vector2 Boundsmax;
    public bool debugBoundBox;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(PlayerOutOfCameraBounds(true));
        //if (PlayerOutOfCameraBounds(true))
        //{
         //   MoveToPlayerXAxis();
        //}
       // if (PlayerOutOfCameraBounds(false))
        //{
       //     MoveToPlayerYAxis();
       // }

    }


    void MoveToPlayerXAxis()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, player.position, ref velocity, smoothSpeed);
        transform.position = new Vector3(targetPos.x, transform.position.y, -10);
    }

    void MoveToPlayerYAxis()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, player.position, ref velocity, smoothSpeed);
        transform.position = new Vector3(transform.position.x, targetPos.y, -10);
    }

    bool PlayerOutOfCameraBounds(bool xAxis)
    {
        if (xAxis)
        {
            if (player.position.x > transform.position.x + Boundsmax.x || player.position.x < transform.position.x - Boundsmin.x)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            if (player.position.y > transform.position.y + Boundsmax.y || player.position.y < transform.position.y - Boundsmin.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }



    void OnDrawGizmosSelected()
    {
        if (debugBoundBox)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(transform.position.x + Boundsmin.x, transform.position.y + Boundsmin.y, -10), new Vector3(transform.position.x + Boundsmin.x, transform.position.y + Boundsmax.y, -10));
            Gizmos.DrawLine(new Vector3(transform.position.x + Boundsmin.x, transform.position.y + Boundsmin.y, -10), new Vector3(transform.position.x + Boundsmax.x, transform.position.y + Boundsmin.y, -10));
            Gizmos.DrawLine(new Vector3(transform.position.x + Boundsmax.x, transform.position.y + Boundsmax.y, -10), new Vector3(transform.position.x + Boundsmin.x, transform.position.y + Boundsmax.y, -10));
            Gizmos.DrawLine(new Vector3(transform.position.x + Boundsmax.x, transform.position.y + Boundsmax.y, -10), new Vector3(transform.position.x + Boundsmax.x, transform.position.y + Boundsmin.y, -10));
        }
    }
}
