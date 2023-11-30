using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private int currentRound = 1;
    [SerializeField] private float roundDuration = 60f; // Set the round duration in seconds
    [SerializeField] private bool buyPhase = true;
    [SerializeField] private float buyPhaseDuration = 15f;
    private float timer;
    [SerializeField] private TextMeshProUGUI UIRoundTimer;
    [SerializeField] private TextMeshProUGUI UIRoundCount;

    private void Start()
    {
        if (photonView.ViewID != 0)
        {
            photonView.ObservedComponents.Add(this);
        }

        if (photonView.IsMine)
        {
            photonView.RPC("StartRound", RpcTarget.All);
            timer = roundDuration; // Initialize the timer
        }
        if(buyPhase)
        {
            timer = buyPhaseDuration;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            timer -= Time.deltaTime;
            UIRoundTimer.text = timer.ToString(); // Update the UI locally
            if (timer <= 0f)
            {
                photonView.RPC("EndRound", RpcTarget.All);
            }
            else
            {
                // Sync the timer across the network
                photonView.RPC("SyncTimer", RpcTarget.Others, timer);
            }
        }
    }

    [PunRPC]
    private void SyncTimer(float syncedTimer)
    {
        // Synchronize the timer for all players
        timer = syncedTimer;
    }

    [PunRPC]
    private void StartRound()
    {
        // Perform any initialization for the start of a round.
    }

    [PunRPC]
    private void EndRound()
    {
        if (buyPhase)
        {
            timer = roundDuration;
            buyPhase = false;
            SetPlayPhase();
            return;
        }
        currentRound++;
        UIRoundCount.text = currentRound.ToString();

        ShowRoundCount();
        Invoke("ShowTimerCount", 3f);
        photonView.RPC("StartRound", RpcTarget.All);
        buyPhase = true;
        timer = buyPhaseDuration;
        SetBuyPhase();
    }

    private void ShowRoundCount()
    {
        UIRoundTimer.gameObject.SetActive(false);
        UIRoundCount.gameObject.SetActive(true);
    }
    private void ShowTimerCount()
    {
        UIRoundTimer.gameObject.SetActive(true);
        UIRoundCount.gameObject.SetActive(false);
    }

    private void SetBuyPhase()
    {
        GameObject shop = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(1).GetChild(3).gameObject;
        shop.SetActive(true);
        PhotonView shopPhotonView = shop.GetComponent<PhotonView>();

        // Now, call the SyncNextSpellsLevel RPC on the ShopUI script
        shopPhotonView.RPC("SyncNextSpellsLevel", RpcTarget.All);
    }

    private void SetPlayPhase()
    {
        GameObject shop = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(1).GetChild(3).gameObject;
        PhotonView shopPhotonView = shop.GetComponent<PhotonView>();

        shopPhotonView.RPC("PlayPhase", RpcTarget.All);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // This player is sending data. Send the current round and timer.
            stream.SendNext(currentRound);
            stream.SendNext(timer);
        }
        else
        {
            // This player is receiving data. Receive the current round and timer.
            currentRound = (int)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();
        }
    }
}
