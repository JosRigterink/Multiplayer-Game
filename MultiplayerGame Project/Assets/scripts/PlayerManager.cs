using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    int kills;
    int deaths;
    //public int maxKills;
    bool maxKillsReached;

    public int killstreakCounter;
    bool killstreakbool;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void Update()
    {
        if (GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>().gameHasEnded == true)
        {
            return;
        }

        if (maxKillsReached == true)
        {
            GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>().gameHasEnded = true;
            Debug.Log("Game ended");
        }
    }

    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        Invoke("CreateController", 3f);
        //CreateController();
        deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        killstreakCounter++;
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
        kills++;
        if (kills == Launcher.Instance.maxKills)
        {
            PV.RPC("RPC_EnableWinscreen", RpcTarget.All);
        }

        if (killstreakCounter == 3)
        {
            Debug.Log("3 Kill Streak!");
            PV.RPC("RPC_KillStreaks", RpcTarget.All);
            //do killstreak stuff here
        }

        if (killstreakCounter == 4)
        {
            Debug.Log("4 Kill Streak!");
            PV.RPC("RPC_KillStreaks", RpcTarget.All);
            //do killstreak stuff here
        }

    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;
        killstreakCounter++;
        Hashtable hash = new Hashtable();
        hash.Add("Kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (kills == Launcher.Instance.maxKills)
        {
            //maxKillsReached = true;
            PV.RPC("RPC_EnableWinscreen", RpcTarget.All);
        }

        if (killstreakCounter == 3)
        {
            Debug.Log("3 Kill Streak!");
            PV.RPC("RPC_KillStreaks", RpcTarget.All);
            //do killstreak stuff here
        }

        if (killstreakCounter == 4)
        {
            Debug.Log("4 Kill Streak!");
            PV.RPC("RPC_KillStreaks", RpcTarget.All);
            //do killstreak stuff here
        }
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }

    [PunRPC]
    void RPC_EnableWinscreen()
    {
        maxKillsReached = true;
        GameOverScript gameOverScript = GameObject.Find("GameOverCanvas").GetComponent<GameOverScript>();
        gameOverScript.endgametext.text = "GameOver max kills reached Winner:" + PV.Owner.NickName;
        gameOverScript.gameHasEnded = true;
    }

    [PunRPC]
    void RPC_KillStreaks()
    {
        //laat zien dat diegene op een killstreak zit
        GameObject uiTimer = GameObject.Find("TimerUI").gameObject;
        uiTimer.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = PV.Owner.NickName + " is on a " + killstreakCounter + " KillStreak!";
        Invoke("HideKillStreakText", uiTimer.GetComponent<Timer>().showKillstreakText);
    }
    void HideKillStreakText()
    {
        GameObject.Find("TimerUI").transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
    }
}
