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
    public GameObject mapCam;
    public GameObject battleTimer;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Canvas>().enabled == true)
        {
            //return;
        }

        if (gameHasEnded == true)
        {
            scoreboard.GetComponent<CanvasGroup>().alpha = 1;
            GameOver();
            //mapCam.SetActive(true);
        }
    }
    void GameOver()
    {
        mapCam.SetActive(true);
        gameObject.GetComponent<Canvas>().enabled = true;
        //endgametext.text = "GameOver max kills reached Winner:" + PhotonNetwork.NickName;
        battleTimer.GetComponent<Timer>().enabled = false;
        scoreboard.GetComponent<CanvasGroup>().alpha = 1;
        Invoke("DestroyAllPlayers", 0.5f);
    }

    void DestroyAllPlayers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
    }
}
