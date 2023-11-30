using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviour
{
    public GhostPlayerMovement ghostPlayerMovement;
    public PlayerMovementSlope playerMovement;
    public bool isDead = true;

    public GameObject cam;

    public string nickname;

    public TextMeshPro nicknameText;

    [SerializeField] private PhotonView photonView;
    [SerializeField] private Transform abilityHolder;


    public void IsLocalPlayer()
    {
       // TPUtilityHolder.gameObject.SetActive(false);

        nicknameText.gameObject.SetActive(false);

        if (isDead)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ghostPlayerMovement.enabled = true;
        }
        else
        {
            playerMovement.enabled = true;
        }
        cam.SetActive(true);

    }


    public void AddPlayerAbility(GameObject _ability)
    {
       PhotonView.Instantiate(_ability,abilityHolder);
    }

    [PunRPC]
    public void SetNickName(string _name)
    {
        nickname = _name;

        nicknameText.text = nickname;
    }
}
