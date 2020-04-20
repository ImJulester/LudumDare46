using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    bool startGame;
    public Camera gameCamera;
    public Player player;
    public GameObject uiCanvas;

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

    }

    // Update is called once per frame
    void Update()
    {
        
        if (startGame)
        {
            startGameValue += Time.deltaTime * startGameSmoothRate;
            thisCamera.orthographicSize = Mathf.Lerp(MenuCameraSize, gameCamera.orthographicSize, startGameValue);
            transform.position = Vector3.Lerp(startPos, gameCamera.transform.position, startGameValue);

            if(startGameValue >= 1)
            {
                gameCamera.enabled = true;
                player.enabled = true;
                Destroy(gameObject);

            }
        }
    }


    public void StartGame()
    {
        startGame = true;
        Destroy(uiCanvas);
    }
}
