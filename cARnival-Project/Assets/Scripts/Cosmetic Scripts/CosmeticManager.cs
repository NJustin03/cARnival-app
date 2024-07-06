using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticManager : MonoBehaviour
{
    public static CosmeticMaterial duckMaterial;
    public static CosmeticMaterial basketballMaterial;
    public static CosmeticParticle archeryParticle;

    public static List<Cosmetic> cosmetics;

    public static List<Cosmetic> ownedArcheryCosmetics;
    public static List<Cosmetic> ownedDuckCosmetics;
    public static List<Cosmetic> ownedBasketballCosmetics;

    private void Awake()
    {
        if (FindObjectsOfType<ModuleManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Function which equips the passed cosmetic and updates the database with that information.
    public static void EquipCosmetic(Cosmetic selectedCosmetic)
    {
        if (selectedCosmetic.game == "duck")
        {
            duckMaterial = (CosmeticMaterial)selectedCosmetic;
        }
        else if (selectedCosmetic.game == "basketball")
        {
            basketballMaterial = (CosmeticMaterial)selectedCosmetic;
        }
        else if (selectedCosmetic.game == "archery")
        {
            archeryParticle = (CosmeticParticle)selectedCosmetic;
        }
        else
        {
            return;
        }
    }


    private IEnumerator RetrieveAllCosmetics()
    {
        yield return StartCoroutine(APIManager.RetrieveUserItems("duck"));
        AddToCosmeticList(APIManager.cosmeticList);
        yield return StartCoroutine(APIManager.RetrieveUserItems("basketball"));
        AddToCosmeticList(APIManager.cosmeticList);
        yield return StartCoroutine(APIManager.RetrieveUserItems("archery"));
        AddToCosmeticList(APIManager.cosmeticList);
    }

    private void AddToCosmeticList(ItemJson[] items)
    {
        foreach (ItemJson item in items)
        {
            int index = cosmetics.FindIndex(cos => cos.itemID == item.itemID);
            cosmetics[index].isPurchased = true;
            cosmetics[index].userItemID = item.userItemID;
            if (item.isWearing == 1)
            {
                EquipCosmetic(cosmetics[index]);
            }
        }
    }
}
