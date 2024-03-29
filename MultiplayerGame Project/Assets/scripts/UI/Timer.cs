using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Timer : MonoBehaviour
{
    float currentTime = 0f;
    public float startingTime = 90f;
    public float showKillstreakText = 5f;
    PhotonView PV;

    [SerializeField] public TMP_Text countdownText;

    void Start()
    {
        currentTime = startingTime;
        //PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if (currentTime < 11)
        {
            countdownText.color = Color.red;
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>().gameHasEnded = true;
            GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>().endgametext.text = "Battle Time Ended";
            //something to end the game;
        }
        //PV.RPC("RPC_TimerUpdate", RpcTarget.All);
        DisplayTime(currentTime);
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
