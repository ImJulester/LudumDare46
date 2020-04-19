using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ColorLerpTest : MonoBehaviour
{

    public List<Color> colors;
    public Light2D light;
    int index = 0;
    float value;
    public float speed;
    public float intensity;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().material.SetVector("_EmissionColor", colors[0] * intensity);
        light.color = colors[0];
    }

    // Update is called once per frame
    void Update()
    {
        Color currentColor;
        if (colors.Count -1 < index)
        {
            index = 0;
        }

        if (colors.Count - 1 == index)
        {
            currentColor = Color.Lerp(colors[index], colors[0], value);
        }
        else
        {
            currentColor = Color.Lerp(colors[index], colors[index + 1], value);
        }
            
        

        value += Time.deltaTime * speed;
        if(value > 1)
        {
            value = 0;
            index++;
        }
        GetComponent<SpriteRenderer>().material.SetVector("_EmissionColor", currentColor * intensity);
        light.color = currentColor;
    }
}
