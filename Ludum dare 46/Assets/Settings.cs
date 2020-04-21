using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public GameObject mainMenu;
    public Slider musicSlider;
    public Slider sfxSlider;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolumes()
    {
        float volume;
        musicMixer.GetFloat("MusicVol", out volume);
        musicSlider.value = volume;
        sfxMixer.GetFloat("sfxVol", out volume);
        sfxSlider.value = volume;


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
    public void Close()
    {
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void Quit()
    {
        Application.Quit();

    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
