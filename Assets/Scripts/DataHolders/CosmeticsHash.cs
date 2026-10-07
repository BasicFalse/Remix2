using GorillaNetworking;
using UnityEngine;
using System.Collections.Generic;

namespace GorillaNetworking
{
    [CreateAssetMenu(fileName = "CosmeticsHash", menuName = "Cosmetics/CosmeticsHash")]
    public class CosmeticsHash : ScriptableObject
    {
        public List<CosmeticItem> allCosmetics;

        public string[] freeCosmetics;

        public CosmeticItem nullItem;

        public List<CosmeticItem> GetFreeItemsByCatagory(string category)
        {
            List<CosmeticItem> items = new();
            for(int i = 0; i < freeCosmetics.Length; i++)
            {
                CosmeticItem item = allCosmetics.Find(x => x.itemName == freeCosmetics[i]);
                switch (item.itemSlot)
                {
                    case "hat":
                        if (category == "hat" || category == "all")
                        { items.Add(item); }
                        break;
                    case "face":
                        if (category == "face" || category == "all")
                        { items.Add(item); }
                        break;
                    case "badge":
                        if (category == "badge" || category == "all")
                        { items.Add(item); }
                        break;
                    case "chest":
                        if (category == "chest" || category == "all")
                        { items.Add(item); }
                        break;
                    case "holdable":
                        if (category == "holdable" || category == "all")
                        { items.Add(item); }
                        break;
                    default:
                        Debug.LogError("UNDEFINED COSMETIC ITEM SLOT, TYPE OF " + item.itemSlot);
                        break;
                }
            }
            return items;
        }

        public CosmeticItem GetItemFromName(string itemName)
        {
            CosmeticItem cosmeticItemVar = allCosmetics.Find(x => x.itemName == itemName);
            
            return cosmeticItemVar;
        }

        public CosmeticItem GetOwnedItemFromName(string itemName)
        {
            CosmeticItem cosmeticItemVar = GetItemFromName(itemName);
            return cosmeticItemVar;
        }

        public bool OwnsItemFromName(string itemName)
        {
            CosmeticItem cosmeticItemVar = GetItemFromName(itemName);
            return true;
        }


        public CosmeticItem NullItem()
        {
            return nullItem;
        }
    }
}