using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage;
    public float firerate;

    private float nextFire;

    [Header("VFX")]
    public GameObject hitVFX;

    public Camera cam;

    [Header("Ammo")]
    public int mag = 5;

    public int ammo = 6;
    public int magAmmo = 6;

    public bool isAutomatic;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletStartPos;
    public float projectileSpeed;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Animation")]
    public Animation anim;
    public AnimationClip reloadAnim;


    [Header("Recoil Settings")]
    [Range(0, 2)]
    public float recoverPercent = 0.7f;
    [Space]
    public float recoilUp = 1f;

    public float recoilBack = 1f;

    public float delayBeforeReload = 0.2f;

    [Header("Audio")]
    public string shootingAudioName;


    private Vector3 originalPosition;
    private Vector3 recoilVelocity;


    private float recoilLength;
    private float recoverLength;

    private bool recoiling;
    public bool recovering;


    private void OnEnable()
    {
        ammoText.text = ammo + "/" + (magAmmo * mag).ToString();

    }

    private void Start()
    {
        ammoText.text = ammo + "/" + (magAmmo * mag).ToString();

        originalPosition = transform.localPosition;

        recoverLength = 1 / firerate * recoverPercent;
    }

    private void Update()
    {
        if (nextFire > 0)
            nextFire -= Time.deltaTime;

        if ((isAutomatic && Input.GetButton("Fire1")) && nextFire <= 0 && ammo > 0 && anim.isPlaying == false || (!isAutomatic && Input.GetButtonDown("Fire1")) && nextFire <= 0 && ammo > 0 && anim.isPlaying == false)
        {
            nextFire = 1 / firerate;
            ammo--;
            ammoText.text = ammo + "/" + (magAmmo * mag).ToString();
            Fire();
        }
        else if (ammo <=0)
        {

            Invoke("Reload", delayBeforeReload);
        }
        if (Input.GetKeyDown(KeyCode.R) && ammo != magAmmo)
        {
            Reload();
        }
        if (recoiling)
        {
            Recoil();
        }
        if (recovering)
        {
            Recover();
        }
    }
    void Reload()
    {
        if(mag > 0 && anim.isPlaying == false)
        {
            anim.Play(reloadAnim.name);
            mag--;
            ammo = magAmmo;
        }
        ammoText.text = ammo + "/" + (magAmmo * mag).ToString();
    }
    void Fire ()
    {

        AudioManager.instance.GetSoundToPlay(shootingAudioName);
        recoiling = true;
        recovering = false;


        
        GameObject _projectile = PhotonNetwork.Instantiate(bulletPrefab.name, bulletStartPos.position, Quaternion.identity);
        _projectile.GetComponent<Rigidbody>().AddForce(bulletStartPos.transform.forward * projectileSpeed, ForceMode.Acceleration);
    }


    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x,originalPosition.y + recoilUp,originalPosition.z - recoilBack);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recover()
    {
        Vector3 finalPosition = originalPosition;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
