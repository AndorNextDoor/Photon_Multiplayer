using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireASpell : MonoBehaviour
{

    
    public int damage;
    public float firerate;



    [Header("Ammo")]
    public int mag = 5;

    public int ammo = 6;
    public int magAmmo = 6;

    public bool isAutomatic;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletStartPos;
    public float projectileSpeed;

    private float nextFire;

    [Header("VFX")]
    public GameObject hitVFX;

    public Camera cam;
    
    
    [Header("UI")]
    public TextMeshProUGUI ammoText;

    [Header("Animation")]
    public Animation anim;
    public AnimationClip reloadAnim;


    public float delayBeforeReload = 0.2f;

    [Header("Audio")]
    public string shootingAudioName;



    private void OnEnable()
    {
        ammoText.text = ammo + "/" + (magAmmo * mag).ToString();

    }

    private void Start()
    {
        ammoText.text = ammo + "/" + (magAmmo * mag).ToString();
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
        else if (ammo <= 0)
        {
            Invoke("Reload", delayBeforeReload);
        }
        if (Input.GetKeyDown(KeyCode.R) && ammo != magAmmo)
        {
            Reload();
        }
    }
    void Reload()
    {
        if (mag > 0 && anim.isPlaying == false)
        {
            anim.Play(reloadAnim.name);
            mag--;
            ammo = magAmmo;
        }
        ammoText.text = ammo + "/" + (magAmmo * mag).ToString();
    }
    void Fire()
    {

        AudioManager.instance.GetSoundToPlay(shootingAudioName);

        GameObject _projectile = PhotonNetwork.Instantiate(bulletPrefab.name, bulletStartPos.position, Quaternion.identity);
        _projectile.GetComponent<Rigidbody>().AddForce(_projectile.transform.forward * projectileSpeed);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;



        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
            if (hit.transform.gameObject.GetComponent<Health>())
            {
                PhotonNetwork.LocalPlayer.AddScore(damage);
                if (damage >= hit.transform.GetComponent<Health>().health)
                {
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                }
                hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }
}
