using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CosmeticManager : MonoBehaviour
{
    public const int defaultDuckID = 110;
    public const int defaultBasketballID = 115;
    public const int defaultArcheryID = 112;

    public static CosmeticMaterial duckMaterial;
    public static CosmeticMaterial basketballMaterial;
    public static CosmeticParticle archeryParticle;

    public static List<Cosmetic> cosmetics;
    public static Dictionary<int, int> userCosmeticInfo; // ItemID/UserItemID pairs.

    public static bool cosmeticsRetrieved = false;

    private void Awake()
    {
        if (FindObjectsOfType<CosmeticManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        userCosmeticInfo = new Dictionary<int, int>();
        LoadAllCosmetics();
        RetrieveUserCosmetics();
    }

    // Function which equips the passed cosmetic and updates the database with that information.
    public void EquipCosmetic(Cosmetic selectedCosmetic)
    {
        int userCosmeticID;
        if (selectedCosmetic.game == "duck")
        {
            duckMaterial = (CosmeticMaterial)selectedCosmetic;
            userCosmeticInfo.TryGetValue(selectedCosmetic.itemID, out userCosmeticID);
            StartCoroutine(APIManager.WearItem(userCosmeticID, 1, 1));
        }
        else if (selectedCosmetic.game == "basketball")
        {
            basketballMaterial = (CosmeticMaterial)selectedCosmetic;
            userCosmeticInfo.TryGetValue(selectedCosmetic.itemID, out userCosmeticID);
            StartCoroutine(APIManager.WearItem(userCosmeticID, 1, 1));
        }
        else if (selectedCosmetic.game == "archery")
        {
            archeryParticle = (CosmeticParticle)selectedCosmetic;
            userCosmeticInfo.TryGetValue(selectedCosmetic.itemID, out userCosmeticID);
            StartCoroutine(APIManager.WearItem(userCosmeticID, 1, 1));
        }
        else
        {
            return;
        }
    }

    // Function to load all cosmetics from the client and store them as a list of references.
    public void LoadAllCosmetics()
    {
        var items = Resources.LoadAll<Cosmetic>("Cosmetics");
        cosmetics = new List<Cosmetic>(items);
    }

    // Public Function which retrieves the player's owned cosmetics from the database.
    public void RetrieveUserCosmetics()
    {
        StartCoroutine(RetrievePlayerCosmetics());
    }

    // Private IEnumerator function which queries the database for each game's owned and equipped cosmetics, and equips default if none owned.
    private IEnumerator RetrievePlayerCosmetics()
    {
        int index = 0;
        yield return StartCoroutine(APIManager.RetrieveUserItems("duck"));
        if (APIManager.cosmeticList.Length == 0)    // If nothing is owned, purchase and equip default.
        {
            index = cosmetics.FindIndex(cos => cos.itemID == defaultDuckID);
            yield return StartCoroutine(APIManager.PurchaseItem(cosmetics[index].itemID, cosmetics[index].game, 1)); // 1 Means to wear in the database.
            AddToOwned(cosmetics[index].itemID);
            EquipCosmetic(cosmetics[index]);
        }
        else
        {
            MarkAsOwned(APIManager.cosmeticList);
        }

        yield return StartCoroutine(APIManager.RetrieveUserItems("basketball"));
        if (APIManager.cosmeticList.Length == 0)    // If nothing is owned, purchase and equip default.
        {
            index = cosmetics.FindIndex(cos => cos.itemID == defaultBasketballID);
            yield return StartCoroutine(APIManager.PurchaseItem(cosmetics[index].itemID, cosmetics[index].game, 1)); // 1 Means to wear in the database.
            AddToOwned(cosmetics[index].itemID);
            EquipCosmetic(cosmetics[index]);
        }
        else
        {
            MarkAsOwned(APIManager.cosmeticList);
        }

        yield return StartCoroutine(APIManager.RetrieveUserItems("archery"));
        if (APIManager.cosmeticList.Length == 0)    // If nothing is owned, purchase and equip default.
        {
            index = cosmetics.FindIndex(cos => cos.itemID == defaultArcheryID);
            yield return StartCoroutine(APIManager.PurchaseItem(cosmetics[index].itemID, cosmetics[index].game, 1)); // 1 Means to wear in the database.
            AddToOwned(cosmetics[index].itemID);
            EquipCosmetic(cosmetics[index]);
        }
        else
        {
            MarkAsOwned(APIManager.cosmeticList);
        }
    }

    // Function to label all owned items and retrieve their userItemIds.
    private void MarkAsOwned(ItemJson[] items)
    {
        foreach (ItemJson item in items)
        {
            int index = cosmetics.FindIndex(cos => cos.itemID == item.itemID);
            userCosmeticInfo.Add(item.itemID, item.userItemID);
            Debug.Log(item.itemID);
            Debug.Log(item.userItemID);
            if (item.isWearing == 1)
            {
                EquipCosmetic(cosmetics[index]);
            }
        }
    }

    // Function which retrieves the item reference.
    public static Cosmetic RetrieveItem(int itemID)
    {
        int index = cosmetics.FindIndex(cos => cos.itemID == itemID);
        if (index == -1)
        {
            return null;
        }
        return cosmetics[index];
    }

    // Function which adds a new item to the owned dict.
    public static void AddToOwned(int itemID)
    {
        userCosmeticInfo.Add(itemID, APIManager.purchase.userItemID);
    }

    // Function which checks to see if the current itemID is equipped anywhere.
    public static bool CheckIfEquipped(int itemID)
    {

        if (duckMaterial.itemID == itemID || basketballMaterial.itemID == itemID || archeryParticle.itemID == itemID)
        {
            return true;
        }
        return false;
    }

    public static int FindEquippedItemID(string game)
    {
        if (game == "duck")
            return duckMaterial.itemID;
        else if (game == "archery")
            return archeryParticle.itemID;
        else 
            return basketballMaterial.itemID;
    }

    public static void ClearCosmeticList()
    {
        duckMaterial = null;
        basketballMaterial = null;
        archeryParticle = null;
        cosmeticsRetrieved = false;
        cosmetics = null;
        userCosmeticInfo.Clear();
    }
}
