using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class AbilitySlam : MonoBehaviourPun
{
    [SerializeField] private string buttonToPress = "Fire3";
    [SerializeField] private float slamRadius = 5f;
    [SerializeField] private float slamMultiplier = 0.5f;
    [SerializeField] private float maxSlamMultiplier = 3f;
    [SerializeField] private float slamForce = 30f;
    [SerializeField] private int blocksAmount = 2;
    [SerializeField] private GameObject stoneToInstantiate;
    private List<GameObject> stones = new List<GameObject>();

    [SerializeField] private Animator animator;

    private Cooldown cooldownManager;
    public AbilitiesSwitcher abilitiesSwitcher;

    private void Awake()
    {
        abilitiesSwitcher = transform.parent.GetComponent<AbilitiesSwitcher>();
        cooldownManager = GetComponent<Cooldown>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (cooldownManager.isOnCooldown)
            return;

        // Check for mouse button press
        if (Input.GetButtonDown(buttonToPress))
        {
            // Start the "HandsUp" animation
            animator.SetTrigger("HandsUpTrigger");
        }

        // Check for mouse button release
        if (Input.GetButtonUp(buttonToPress))
        {
            // Start the "HitGround" animation
            animator.SetTrigger("HitGroundTrigger");
            StartCoroutine(Slam());
        }
        if (Input.GetButton(buttonToPress))
        {
            slamMultiplier += Time.deltaTime;
        }
    }

    IEnumerator Slam()
    {
        if (slamMultiplier > maxSlamMultiplier)
            slamMultiplier = maxSlamMultiplier;
        cooldownManager.isOnCooldown = true;
        cooldownManager.SetOnCooldown();

        for (int i = 0; i < blocksAmount * slamMultiplier; i++)
        {
            // Use the player's forward direction as the starting direction
            Vector3 startDirection = transform.forward;

            // Use PhotonNetwork.Instantiate to ensure network synchronization
            GameObject stone = PhotonNetwork.Instantiate(stoneToInstantiate.name, transform.position + startDirection * (1.5f * i + 1), transform.rotation);
            stone.transform.localScale += new Vector3(2f * i + 1, 0, 0);
            stones.Add(stone);
            stone.GetComponent<Rigidbody>().AddForce(Vector3.up * slamForce * slamMultiplier, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);

        }

        StartCoroutine(DestroyStones());
        ShowBareHands();
    }
    IEnumerator DestroyStones()
    {
        List<GameObject> stonesCopy = new List<GameObject>(stones);

        slamMultiplier = 0.5f;
        foreach (var stone in stonesCopy)
        {
            PhotonNetwork.Destroy(stone);
            stones.Remove(stone);
            yield return new WaitForSeconds(0.4f);
        }
    }

    void ShowBareHands()
    {
        abilitiesSwitcher.ShowHands();
    }
}