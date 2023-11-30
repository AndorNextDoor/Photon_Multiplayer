using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    [SerializeField] private Item[] items;
    [SerializeField] private GameObject player;
    [SerializeField] private int spellIndex = 0;
    [SerializeField] private int maxSpellIndex = 1;
    [SerializeField] private Health health;


    [Header("AbilitiesUI")]
    [SerializeField] private Transform abilityContainer;

    private void Awake()
    {
        container = transform;
        shopItemTemplate = container.GetChild(0);
        shopItemTemplate.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Call CreateItemButton for each item in the array
        foreach (Item item in items)
        {
            if (item.abilityLevel == spellIndex)
            {
                CreateItemButton(item);
            }
        }
        Invoke("CheckNumberOfItemsInShop",1f);
    }
    public void NextSpellsLevel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(i >= 1)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        spellIndex++;
        if (spellIndex > maxSpellIndex)
        {
            spellIndex--;
            DisableShop();
        }
        foreach (Item item in items)
        {
            if (item.abilityLevel == spellIndex)
            {
                CreateItemButton(item);
            }
        }
        Invoke("CheckNumberOfItemsInShop", 1f);

    }

    [PunRPC]
    private void SyncNextSpellsLevel()
    {
        maxSpellIndex++;
        if (maxSpellIndex > 3)
        {
            DisableShop();
            return;
        }
        health.ResetPlayer();
        DisablePlayer();
        NextSpellsLevel();
    }
    [PunRPC]
    private void PlayPhase()
    {
        EnablePlayer();
    }
    void DisablePlayer()
    {
        health.enabled = false;
        health.gameObject.GetComponent<PlayerMovementSlope>().enabled = false;
        health.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }
    void EnablePlayer()
    {
        health.enabled = true;
        health.gameObject.GetComponent<PlayerMovementSlope>().enabled = true;
        health.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CreateItemButton(Item item)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        // Access the properties of the referenced ScriptableObject
        shopItemTransform.Find("itemName").GetComponent<TextMeshProUGUI>().text = item.name;

        // Add a listener to the button to call the appropriate method with the item's ability
        Button itemButton = shopItemTransform.GetComponent<Button>();
        itemButton.onClick.AddListener(() => PurchaseItem(item));
        itemButton.onClick.AddListener(() => NextSpellsLevel());
        shopItemTransform.gameObject.SetActive(true);
    }

    private void PurchaseItem(Item item)
    {

        Transform abilityUI = Instantiate(abilityContainer, abilityContainer.parent);
        abilityUI.gameObject.SetActive(true);
        abilityUI.GetChild(0).GetChild(1).GetComponent<Image>().sprite = item.abilitySprite;
        // Call your RPC method with the item's ability
        item.ability.GetComponent<Cooldown>().cooldownSlider = abilityUI.GetChild(0).GetComponent<Slider>();
        item.ability.GetComponent<Cooldown>().needsUI = true;
        player.GetComponent<PlayerSetup>().AddPlayerAbility(item.ability);
    }

    private void CheckNumberOfItemsInShop()
    {
        if (transform.childCount == 1)
            DisableShop();
    }

    private void DisableShop()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        this.gameObject.SetActive(false);
    }
}
