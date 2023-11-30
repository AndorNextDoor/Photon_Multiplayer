using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitySwitcher : MonoBehaviour
{
    public PhotonView playerSetupView;

    private int selectedUtility = 0;
    private bool hideUtility;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            selectedUtility = 0;
            hideUtility = false;
            ShowUtility();
        }
        if (Input.GetKey(KeyCode.E))
        {
            selectedUtility = 1;
            hideUtility = false;
            ShowUtility();
        }



        if (Input.GetKeyUp(KeyCode.Q))
        {
            selectedUtility = 0;
            hideUtility = true;
            Invoke("HideUtility", 0.1f);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            selectedUtility = 1;
            hideUtility = true;
            Invoke("HideUtility",0.1f);
        }
    }



    void ShowUtility()
    {
        playerSetupView.RPC("SetTPUtility", RpcTarget.All, selectedUtility,hideUtility);
        if(selectedUtility >= transform.childCount)
        {
            selectedUtility = transform.childCount - 1;
        }
        int i = 0;

        foreach (Transform _utility in transform)
        {
            if (i == selectedUtility)
            {
                _utility.gameObject.SetActive(true);
            }
            else
            {
                _utility.gameObject.SetActive(false);
            }
            i++;
        }
    }
    void HideUtility()
    {
        playerSetupView.RPC("SetTPUtility", RpcTarget.All, selectedUtility, hideUtility);
        foreach (Transform _utility in transform)
        {
            _utility.gameObject.SetActive(false);
        }
    }
}
