using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    public int damage;
    public float explosionRange;
    public LayerMask explosionLayer;
    public GameObject anim;


    private void Start()
    {
        Invoke("EnableCollision",0.1f);
        Invoke("DestroyProjectile", 3f);
    }
    void DestroyProjectile()
    {
        Destroy(gameObject);


    }
    void EnableCollision()
    {
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        PhotonNetwork.Instantiate(anim.name, transform.position, Quaternion.identity);
        Explode();
        DestroyProjectile();


    }
    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange,explosionLayer);

        foreach (Collider hitCollider in colliders)
        {
            if (hitCollider.transform.gameObject.GetComponent<Health>())
            {
                PhotonNetwork.LocalPlayer.AddScore(damage);
                if (damage >= hitCollider.transform.GetComponent<Health>().health)
                {
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                }
                hitCollider.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.Others, damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a wire sphere in the editor to visualize the explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
