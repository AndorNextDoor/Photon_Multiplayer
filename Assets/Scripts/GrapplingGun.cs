using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GrapplingGun : MonoBehaviour
{
    private Vector3 grapplePoint;

    public LayerMask grappable;
    public Transform gunTip, cam, TPGunTip;
    public Transform player;


    private SpringJoint joint;


    private PhotonView photonView;

    [Header("Grapple settings")]
    public float grappleTimer = 3.5f;
    private bool isGrappled;
    private float timeToGrapple;
    private Cooldown cooldownManager;
    [Space]
    public float maxDistance = 100f;
    private LineRenderer lineRenderer;
    public float minGrappleDistanceMultiprier = 0.25f;
    public float maxGrappleDistanceMultiprier = 0.8f;
    [Space]
    public float spring = 4.5f;
    public float damper = 7f;
    public float grappleMass = 4.5f;
    [Space]
    public AbilitiesSwitcher abilitiesSwitcher;



    private void Awake()
    {
        player = transform.parent.parent.parent;
        cam = transform.parent.parent;
        abilitiesSwitcher = transform.parent.GetComponent<AbilitiesSwitcher>();

        cooldownManager = this.GetComponent<Cooldown>();
        lineRenderer = gunTip.GetComponent<LineRenderer>();
        StopGrapple();
        photonView = gameObject.GetComponent<PhotonView>();
        timeToGrapple = grappleTimer;
    }
    private void Update()
    {
        if (isGrappled)
        {
            timeToGrapple -= Time.deltaTime;
            if(timeToGrapple < 0)
            {
                StopGrapple();
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            StopGrapple();
        }
        if (Input.GetKeyDown(KeyCode.Q) && !cooldownManager.isOnCooldown)
        {
            if (abilitiesSwitcher.isUsingAbility)
                return;
            StartGrapple();
        }



    }
    private void LateUpdate()
    {
        DrawRope();
  //      photonView.RPC("TPDrawRope", RpcTarget.All);
    }
    void StartGrapple ()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position,cam.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point; 
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position,grapplePoint);

            joint.maxDistance = distanceFromPoint * maxGrappleDistanceMultiprier;
            joint.minDistance = distanceFromPoint * minGrappleDistanceMultiprier;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = grappleMass;

            lineRenderer.positionCount = 2;

            isGrappled = true;
            cooldownManager.SetOnCooldown();
        }
        else
        {

        }
    }
    void StopGrapple()
    {
        timeToGrapple = grappleTimer;
        lineRenderer.positionCount = 0;
        isGrappled = false;
        Destroy(joint);
        abilitiesSwitcher.ShowHands();
    }
    void DrawRope()
    {
        if (!joint)
            return;
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
  /*  [PunRPC]
    void TPDrawRope()
    {
        if (!joint)
            return;
        lineRenderer.SetPosition(0, TPGunTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
*/

}
