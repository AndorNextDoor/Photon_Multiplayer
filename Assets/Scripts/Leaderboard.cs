using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Unity.VisualScripting;

public class Leaderboard : MonoBehaviour
{

    public GameObject playersHolder;
    public GameObject playersHolderNames;

    [Header("Options")]
    public float refreshRate = 1f;


    [Header("UI")]
    public GameObject[] slots;


    // Start is called before the first frame update
    void Start()
    {
     InvokeRepeating(nameof(Refresh),1f, refreshRate);   
    }

    public void Refresh()
    {
        foreach(var slot in slots)
        {
            slot.SetActive(false);
        }
        var _sortedPlayerList =
            (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;
        foreach(var player in _sortedPlayerList)
        {
            slots[i].SetActive(true);
            if (player.NickName == "")
                player.NickName = "Unnamed";

            TextMeshProUGUI _nameText = slots[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI _scoreTexts = slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI _KDTexts = slots[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            _nameText.text = player.NickName;
            _scoreTexts.text = player.GetScore().ToString();


            if (player.CustomProperties["kills"] != null)
            {
                _KDTexts.text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"];
            }
            else
            {
                _KDTexts.text = "0/0";
            }

            i++;
        }
    }
    void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
        playersHolderNames.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
