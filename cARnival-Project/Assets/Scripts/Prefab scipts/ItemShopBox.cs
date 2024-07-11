using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemShopBox : MonoBehaviour
{
    public Cosmetic item;
    public int itemID;
    public GameObject costBox;
    public TextPrefabScript costText;

    public GameObject statusBox;
    public TextPrefabScript statusText;

    public Image itemDisplay;
    public CosmeticManager cosmeticManager;

    public bool isOwned;
    public bool isEquipped;

    public ItemTab parentTab;
    public Button itemShopButton;

    void Awake()
    {
        cosmeticManager = FindAnyObjectByType<CosmeticManager>();
        item = CosmeticManager.RetrieveItem(itemID);
        if (item == null )
        {
            Debug.Log("No Item Found");
            return;
        }
        itemDisplay.sprite = item.icon;
        if (CosmeticManager.userCosmeticInfo.ContainsKey(itemID))
        {
            isOwned = true;
            costBox.SetActive(false);
            if (CosmeticManager.CheckIfEquipped(itemID))
            {
                statusText.Text = "Equipped";
                isEquipped = true;
            }
            else
            {
                statusText.Text = "Owned";
                isEquipped = false;
            }
            statusBox.SetActive(true);
            SetButtonToEquip();
        }
        else
        {
            isOwned = false;
            costText.Text = item.cost.ToString();
        }
    }

    public IEnumerator PurchaseItem()
    {
        yield return StartCoroutine(APIManager.PurchaseItem(item.itemID, item.game, 0));
        CosmeticManager.AddToOwned(item.itemID);
        statusText.Text = "Owned";
        isOwned = true;
        costBox.SetActive(false);
        statusBox.SetActive(true);
        SetButtonToEquip();
    }

    public void SetToOwned()
    {
        isEquipped = false;
        statusText.Text = "Owned";
    }

    public void SetToEquipped()
    {
        parentTab.SetBoxesToOwned(this);
        isEquipped = true;
        statusText.Text = "Equipped";
        cosmeticManager.EquipCosmetic(item);
    }

    private void SetButtonToEquip()
    {
        for (int i = 0; i < itemShopButton.onClick.GetPersistentEventCount(); i++)
        {
            itemShopButton.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
        itemShopButton.onClick.AddListener(SetToEquipped);
    }
}
