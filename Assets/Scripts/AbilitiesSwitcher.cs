using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesSwitcher : MonoBehaviour
{
    private int selectedAbility;
    public bool isUsingAbility = false;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void Update()
    {
            if (isUsingAbility)
                return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShowAbility(1);
        }
        if (Input.GetButton("Fire1"))
        {
            ShowAbility(2);
        }
        if (Input.GetButton("Fire2"))
        {
            ShowAbility(3);
        }
        if (Input.GetButton("Fire3"))
        {
            ShowAbility(4);
        }
        //more abilities
    }
    void ShowAbility(int _selectedAbility)
    {
        if (_selectedAbility > transform.childCount - 1)
            return;
        if (transform.GetChild(_selectedAbility).GetComponent<Cooldown>().isOnCooldown)
            return;
        selectedAbility = _selectedAbility;

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(selectedAbility).GetChild(0).gameObject.SetActive(true);
        Invoke("AbilityInUse", .1f);

    }
    void AbilityInUse()
    {
        isUsingAbility = true;
    }
    public void ShowHands()
    {
        isUsingAbility = false;
        transform.GetChild(selectedAbility).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
