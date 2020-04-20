using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public GameObject mainMenu;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void MusicSliderChange(float value)
    {
        musicMixer.SetFloat("MusicVol", value);
    }
    public void VfxSliderChange(float value)
    {
        sfxMixer.SetFloat("sfxVol", value);
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
