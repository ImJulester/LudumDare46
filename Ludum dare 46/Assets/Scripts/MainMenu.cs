﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    bool startGame;
    public Camera gameCamera;
    public Player player;
    public GameObject secondCanvas;
    public GameObject mainMenuCanvas;
    public GameObject settings;

    public bool instaStart;

    public float startGameSmoothRate = 1f;

    float startGameValue;
    Camera thisCamera;

    float MenuCameraSize;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        thisCamera = GetComponent<Camera>();
        MenuCameraSize = thisCamera.orthographicSize;
        startPos = transform.position;
        secondCanvas.SetActive(false);

        if (instaStart)
        {
            StartGame();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (startGame)
        {
            startGameValue += Time.deltaTime * startGameSmoothRate;
            thisCamera.orthographicSize = Mathf.Lerp(MenuCameraSize, gameCamera.orthographicSize, startGameValue);
            transform.position = Vector3.Lerp(startPos, gameCamera.transform.position, startGameValue);

            if (startGameValue >= 1)
            {
                gameCamera.enabled = true;
                player.enabled = true;
                secondCanvas.SetActive(true);
                Destroy(gameObject);

            }
        }
    }


    public void StartGame()
    {
        startGame = true;
        Destroy(mainMenuCanvas);
    }

    public void Settings()
    {
        settings.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

}
