using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour
{
    public GameObject[] waterfallDisapear;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < waterfallDisapear.Length; i++)
        {
            waterfallDisapear[i].SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < waterfallDisapear.Length; i++)
        {
            waterfallDisapear[i].SetActive(true);
        }
    }
}
