using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class Projectile : MonoBehaviour
{

    public int damage;
    public float explosionRange;
    public LayerMask explosionLayer;
    public GameObject anim;
    [Header("Explosion force")]
    public bool needToPush = false;
    public float explosionForce = 10f;

    private PhotonView photonView;

    private bool exploded = false;


    private void OnEnable()
    {
        photonView = GetComponent<PhotonView>();
        Invoke("DestroyProjectile", 3f);
    }
    void DestroyProjectile()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider collision)
    {

        PhotonView _photonView = collision.gameObject.GetComponent<PhotonView>();
        if (_photonView != null)
        {
            if (_photonView.IsMine)
                return;
        }
        PhotonNetwork.Instantiate(anim.name, transform.position, Quaternion.identity);
        Explode();
        DestroyProjectile();
        
    }
    private void Explode()
    {


        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange, explosionLayer);

        foreach (Collider hitCollider in colliders)
        {
            if (hitCollider.transform.gameObject.GetComponent<Health>())
            {
                DamagePlayer(hitCollider.transform);
                if (!needToPush)
                    return;
                PushPlayer(hitCollider.transform);
            }
        }
    }

    void PushPlayer(Transform player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            rb.AddForce(dir.normalized * explosionForce, ForceMode.Impulse);
        }
    }

    void DamagePlayer(Transform player)
    {
        PhotonNetwork.LocalPlayer.AddScore(damage);
        if (damage >= player.transform.GetComponent<Health>().health)
        {
            RoomManager.instance.kills++;
            RoomManager.instance.SetHashes();
        }
        player.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
    }

    private void OnDrawGizmos()
    {
        // Draw a wire sphere in the editor to visualize the explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
