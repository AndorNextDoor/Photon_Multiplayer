using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{

    public string buttonToShoot = "Fire1";
    public AbilitiesSwitcher abilitiesSwitcher;


    [Header("Cooldown")]
    public bool isAutomatic;
    private Cooldown cooldownManager;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform projectileStartPos;
    public float projectileSpeed;

    [Header("Animation")]
    public Animation shootAnim;

    [Header("Audio")]
    public string shootingAudioName;


    private void Awake()
    {
        abilitiesSwitcher = transform.parent.GetComponent<AbilitiesSwitcher>();
        cooldownManager = GetComponent<Cooldown>();
    }

    private void Update()
    {

        if ((isAutomatic && Input.GetButton(buttonToShoot)) && shootAnim.isPlaying == false && !cooldownManager.isOnCooldown || (!isAutomatic && Input.GetButtonDown(buttonToShoot)) && shootAnim.isPlaying == false && !cooldownManager.isOnCooldown)
        {
            if (abilitiesSwitcher.isUsingAbility)
                return;
            PlayAnimation();
        }
    }


    void PlayAnimation()
    {
        shootAnim.Play();
    }
    void OnAnimationEnd()
    {
        cooldownManager.isOnCooldown = true;
        cooldownManager.SetOnCooldown();
        AudioManager.instance.GetSoundToPlay(shootingAudioName);
        GameObject _projectile = PhotonNetwork.Instantiate(projectilePrefab.name, projectileStartPos.position, Quaternion.identity);
        _projectile.GetComponent<Rigidbody>().AddForce(projectileStartPos.transform.forward * projectileSpeed, ForceMode.Acceleration);
    }

    void ShowBareHands()
    {
        abilitiesSwitcher.ShowHands();
    }
}
