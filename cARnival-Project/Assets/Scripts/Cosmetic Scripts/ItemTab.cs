using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTab : MonoBehaviour
{
    public ItemShopBox[] itemShopBoxes;

    public void SetBoxesToOwned(ItemShopBox itemToEquip)
    {
        foreach (ItemShopBox item in itemShopBoxes)
        {
            if (item.itemID == itemToEquip.itemID)
                continue;

            if (item.isOwned)
            {
                item.SetToOwned();
            }
        }
    }

}
