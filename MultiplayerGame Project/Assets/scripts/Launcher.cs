using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class MapData
{
    public string name;
    public int scene;
}

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    public int maxKills;
    public MapData[] maps;
    public int currentmap = 0;
    [SerializeField] TMP_Text mapValue;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject isHostObject;
    [SerializeField] GameObject maxKillText;

    [SerializeField] float currentCountdown = 5f;
    [SerializeField] bool startTimer;
    [SerializeField] TMP_Text startgameCounter;
    [SerializeField] int gameTimeInSec;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    void Update()
    {
        if (startTimer)
        {
            currentCountdown -= 1 * Time.deltaTime;
            startgameCounter.text = currentCountdown.ToString("0");

            if (currentCountdown <= 0)
            {
                currentCountdown = 0;
                PhotonNetwork.LoadLevel(maps[currentmap].scene);
                startTimer = false;
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }
    
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        Hashtable options = new Hashtable();
        options.Add("Time", gameTimeInSec);
        roomOptions.CustomRoomProperties = options;

        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListIten>().SetUp(players[i]);
        }

        isHostObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        isHostObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed:" + message;
        MenuManager.Instance.OpenMenu("error");
    }
    public void ChangeMap()
    {
        currentmap++;
        if (currentmap >= maps.Length) currentmap = 0;
        mapValue.text = "MAP: " + maps[currentmap].name;
    }

    public void StartGame()
    {
        startTimer = true;
        //PhotonNetwork.LoadLevel(1);
        //PhotonNetwork.LoadLevel(maps[currentmap].scene);
    }
    public void leaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListIten>().SetUp(newPlayer);
    }

    public void ChangeMaxKills()
    {
        maxKills += 5;
        maxKillText.GetComponent<TMP_Text>().text = "Maxkills: " + maxKills.ToString();
    }

    public void DecreaseMaxKills()
    {
        maxKills -= 5;
        maxKillText.GetComponent<TMP_Text>().text = "Maxkills: " + maxKills.ToString();
        if (maxKills <= 5)
        {
            maxKills = 5;
            maxKillText.GetComponent<TMP_Text>().text = "Maxkills: " + maxKills.ToString();
        }
    }
}
