using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Timer : MonoBehaviour
{
    public float showKillstreakText = 5f;

    public TMP_Text countdownText;
    public bool count;
    public int Time;
    ExitGames.Client.Photon.Hashtable setTime = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        count = true;
    }

    private void Update()
    {
        Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
        float minutes = Mathf.FloorToInt((int)PhotonNetwork.CurrentRoom.CustomProperties["Time"] / 60);
        float seconds = Mathf.FloorToInt((int)PhotonNetwork.CurrentRoom.CustomProperties["Time"] % 60);


        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (PhotonNetwork.IsMasterClient)
        {
            if (count)
            {
                count = false;
                StartCoroutine(timer());
            }
        }

        if (Time < 11)
        {
            countdownText.color = Color.red;
        }

        if (Time < 0)
        {
            Time = 0;
            countdownText.text = "00:00";
            GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>().gameHasEnded = true;
            GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>().endgametext.text = "Battle Time Ended";
        }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        int nexttime = Time -= 1;
        setTime["Time"] = nexttime;
        PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);

        count = true;
    }
}
