using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Health : MonoBehaviour
{
    public int health;
    public bool isLocalPlayer;
    [SerializeField] private int maxHealth = 100;
    private int deathsAmount = 0;


    [Header("UI")]
    public TextMeshProUGUI healthText;


    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        healthText.text = health.ToString();
        if(health <= 0)
        {
            if(isLocalPlayer)
            {
                deathsAmount++;
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();

                health = maxHealth;
                healthText.text = health.ToString();
                RoomManager.instance.ResetPlayer(transform);
                if(deathsAmount == 10)
                {
                    PhotonNetwork.Destroy(gameObject);
                    RoomManager.instance.SpawnGhostPlayer();
                }
            }
        }
    }

    public void ResetPlayer()
    {
        RoomManager.instance.ResetPlayer(transform);
        health = maxHealth;
        healthText.text = health.ToString();
    }

}
