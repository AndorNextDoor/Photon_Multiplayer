using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingFloor : MonoBehaviour
{
    public float damageInterval = 1.5f;
    private float interval;
    public int damage = 10;


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Health>())
        {
            if (interval <= 0)
            {
                collision.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
                interval = damageInterval;
            }
        }
    }


    private void Update()
    {
        if(interval > 0)
            interval -= Time.deltaTime;
    }
}
