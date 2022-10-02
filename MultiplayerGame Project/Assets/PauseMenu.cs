using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool gameIsPaused = false;
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Pause();
            }
        }
    }
    void Resume()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Continue()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void options()
    {
        //put some options things here
        Debug.Log("Pressed Options");
    }

    public void Leave()
    {
        Debug.Log("QuitGame");
        Application.Quit();
        //selution for now^
    }
}
