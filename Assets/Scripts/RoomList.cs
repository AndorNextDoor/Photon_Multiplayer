using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList instance; 

    public GameObject roomManagerGameObject;
    public RoomManager roomManager;


    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;
    [Space]
    public GameObject roomListScreen;
    public GameObject connectingScreen;
    [Space]
    public TextMeshProUGUI playerName;



    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();


    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CheckNicnname();
    }
    public void SetNickname(string _name)
    {
        PlayerPrefs.SetString("PlayerName", _name);
        playerName.text = _name;
    }
    public void CheckNicnname()
    {
        string _name = PlayerPrefs.GetString("PlayerName");
        if (_name == "")
        {
            playerName.text = "Unnamed";
            return;
        }
        playerName.text = _name;
    }
    
    
    public void ChangeRoomToCreateName(string _roomName)
    {
        roomManager.roomNameToJoin = _roomName;
    }


    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Precautions
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        roomListScreen.SetActive(true);
        connectingScreen.SetActive(false);
        
        PhotonNetwork.JoinLobby();

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        if(cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for(int i = 0;i < cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;

                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;

                    }
                }
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach(Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }
        foreach(var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/16";

            roomItem.GetComponent<RoomItemButton>().roomName = room.Name;
        }
    }
    public void JoinRoomByName(string name)
    {
        roomManager.roomNameToJoin = name;
        roomManager.gameObject.SetActive(true);
        roomManager.JoinRoomButtonPressed();
        gameObject.SetActive(false);
    }
}
