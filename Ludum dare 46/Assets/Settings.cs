using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer musicMixer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void MusicSliderChange(float value)
    {
        musicMixer.SetFloat("MusicVol", Mathf.Log10(value * 20));
    }
    public void VfxSliderChange(float value)
    {
        Debug.Log("value : " + value);
    }
}
