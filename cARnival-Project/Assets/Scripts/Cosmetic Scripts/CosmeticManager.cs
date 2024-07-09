using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CosmeticManager : MonoBehaviour
{
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
        LoadAllCosmetics();
        RetrieveUserCosmetics();
        // Insert condition if defaults aren't owned to purchase/ID them.
    }

    // Function which equips the passed cosmetic and updates the database with that information.
    public void EquipCosmetic(Cosmetic selectedCosmetic)
    {
        if (selectedCosmetic.game == "duck")
        {
            duckMaterial = (CosmeticMaterial)selectedCosmetic;
            StartCoroutine(APIManager.WearItem(duckMaterial.userItemID, true, true));
        }
        else if (selectedCosmetic.game == "basketball")
        {
            basketballMaterial = (CosmeticMaterial)selectedCosmetic;
            StartCoroutine(APIManager.WearItem(basketballMaterial.userItemID, true, true));
        }
        else if (selectedCosmetic.game == "archery")
        {
            archeryParticle = (CosmeticParticle)selectedCosmetic;
            StartCoroutine(APIManager.WearItem(archeryParticle.userItemID, true, true));
        }
        else
        {
            return;
        }
    }

    public void LoadAllCosmetics()
    {
        var items = Resources.LoadAll<Cosmetic>("Cosmetics");
        cosmetics = new List<Cosmetic>(items);
    }

    public void RetrieveUserCosmetics()
    {
        StartCoroutine(RetrievePlayerCosmetics());
    }

    private IEnumerator RetrievePlayerCosmetics()
    {
        yield return StartCoroutine(APIManager.RetrieveUserItems("duck"));
        MarkAsOwned(APIManager.cosmeticList);

        yield return StartCoroutine(APIManager.RetrieveUserItems("basketball"));
        MarkAsOwned(APIManager.cosmeticList);

        yield return StartCoroutine(APIManager.RetrieveUserItems("archery"));
        MarkAsOwned(APIManager.cosmeticList);
    }

    private void MarkAsOwned(ItemJson[] items)
    {
        userCosmeticInfo = new Dictionary<int, int>();
        foreach (ItemJson item in items)
        {
            int index = cosmetics.FindIndex(cos => cos.itemID == item.itemID);
            userCosmeticInfo.Add(item.itemID, item.userItemID);
            if (item.isWearing == 1)
            {
                EquipCosmetic(cosmetics[index]);
            }
        }
    }

    public static Cosmetic RetrieveItem(int itemID)
    {
        int index = cosmetics.FindIndex(cos => cos.itemID == itemID);
        if (index == -1)
        {
            return null;
        }
        return cosmetics[index];
    }

    public static void AddToOwned(int itemID)
    {
        userCosmeticInfo.Add(itemID, APIManager.purchase.userItemID);
    }

    private static void ClearCosmeticList()
    {
        duckMaterial = null;
        basketballMaterial = null;
        archeryParticle = null;
        cosmeticsRetrieved = false;
    }
}
