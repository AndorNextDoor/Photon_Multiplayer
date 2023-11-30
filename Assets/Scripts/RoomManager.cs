using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;


    public GameObject player;
    public GameObject playerGhost;
    public string playerID;

    [Space]
    public Transform[] spawnPoints;

    [Space]
    public GameObject roomCam;

    private string nickname = "Unnamed";
    public GameObject nameUI;
    public GameObject connectingUI;
    [SerializeField] private GameObject roundManager;

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;


    public string roomNameToJoin = "test";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if(PlayerPrefs.GetString("PlayerName") != "")
        {
            nickname = PlayerPrefs.GetString("PlayerName");
        }
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");

        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin,null,null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("We're in the room");


        roomCam.SetActive(false);
        SpawnPlayer();
        roundManager.SetActive(true);
    }

    public void ResetPlayer(Transform player)
    {
        player.transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;
    }
    public void SpawnPlayer()
    {
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoints[UnityEngine.Random.Range(0,spawnPoints.Length)].position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;

        _player.GetComponent<PhotonView>().RPC("SetNickName",RpcTarget.AllBuffered,nickname);

        PhotonNetwork.LocalPlayer.NickName = nickname;
    }
    public void SpawnGhostPlayer()
    {
        GameObject _player = PhotonNetwork.Instantiate(playerGhost.name, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<PhotonView>().RPC("SetNickName", RpcTarget.AllBuffered, nickname);
    }
    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {

        }
    }
}
