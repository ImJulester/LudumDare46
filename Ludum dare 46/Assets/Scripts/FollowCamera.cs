using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Transform player;

    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;

    public Vector2 Boundsmin;
    public Vector2 Boundsmax;
    public bool debugBoundBox;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);
        targetPos = new Vector3(player.position.x, player.position.y, -10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        SetTargetPos();

        MoveToPlayer();
    }


    void MoveToPlayer()
    {
        Vector3 pos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothSpeed);
        transform.position = new Vector3(pos.x, pos.y, -10);
    }

    void SetTargetPos()
    {
        //X
        if (player.position.x > transform.position.x + Boundsmax.x)
        {
            float x  = player.position.x - (transform.position.x + Boundsmax.x);
            targetPos.x = transform.position.x + x;
        }
        if (player.position.x < transform.position.x + Boundsmin.x)
        {
            float x = player.position.x - (transform.position.x + Boundsmin.x);
            targetPos.x = transform.position.x + x;
        }
        if (player.position.y > transform.position.y + Boundsmax.y)
        {
            float y = player.position.y - (transform.position.y + Boundsmax.y);
            targetPos.y = transform.position.y + y;
        }
        if (player.position.y < transform.position.y + Boundsmin.y)
        {
            float y = player.position.y - (transform.position.y + Boundsmin.y);
            targetPos.y = transform.position.y + y;
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
