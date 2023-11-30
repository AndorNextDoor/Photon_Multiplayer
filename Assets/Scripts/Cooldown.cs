using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public float cooldown = 5f;
    private float cooldownTime;
    public bool isOnCooldown = true;

    
    public bool needsUI = true;
    public Slider cooldownSlider;

    private void Start()
    {
        cooldownTime = cooldown;
        if (!needsUI)
            return;
        cooldownSlider.maxValue = cooldown;
    }

    public void SetOnCooldown()
    {
        isOnCooldown = true;
        cooldown = cooldownTime;
    }

    private void Update()
    {
        if(cooldown>0)
        {
            cooldown -= Time.deltaTime;
            if (!needsUI)
                return;
            cooldownSlider.value = cooldown;
        }
        else
        {
            isOnCooldown = false;
        }
    }

}
