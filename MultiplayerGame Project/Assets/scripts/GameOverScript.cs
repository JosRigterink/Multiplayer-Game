using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameOverScript : MonoBehaviour
{
    public bool gameHasEnded;
    public GameObject scoreboard;
    public TMP_Text endgametext;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Canvas>().enabled == true)
        {
            //return;
        }

        if (gameHasEnded == true)
        {
            GameOver();
        }

        
    }
    void GameOver()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        scoreboard.GetComponent<CanvasGroup>().alpha = 1;
        endgametext.text = "GameOver Max kills reached";
    }
}
