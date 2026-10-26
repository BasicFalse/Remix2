using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using GorillaNetworking;
using GorillaRigging;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
public class CosmeticsController : MonoBehaviour
{

    [Serializable]
    public struct CosmeticSet
    {
        public CosmeticItem hat;

        public CosmeticItem badge;

        public CosmeticItem face;

        public CosmeticItem chest;

        public CosmeticItem holdable;
    }


    public static volatile CosmeticsController instance;

    public List<CosmeticItem> allCosmetics = new();
    public CosmeticItem nullItem;
    public GorillaComputer computer;
    public CosmeticsHash cosmeticsHash;
    public CosmeticSet tryOnSet;
    public List<CosmeticStand> cosmeticStands;
    public List<Wardrobe> wardrobes;
    public List<CosmeticItem> unlockedCosmetics;
    public List<int> cosmeticsPages = new();
    public List<CosmeticItem>[] itemLists = new List<CosmeticItem>[5];
    private int wardrobeType;
    public CosmeticSet currentWornSet;
    private CosmeticItem cosmeticItemVar;
    private string returnString;
    private string[] returnStringArray = new string[5];

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
        nullItem = cosmeticsHash.NullItem();
        tryOnSet.hat = nullItem;
        tryOnSet.badge = nullItem;
        tryOnSet.face = nullItem;
        tryOnSet.chest = nullItem;
        tryOnSet.holdable = nullItem;

        cosmeticsPages.Add(0);cosmeticsPages.Add(0);cosmeticsPages.Add(0);cosmeticsPages.Add(0);cosmeticsPages.Add(0);
        unlockedCosmetics = cosmeticsHash.GetFreeItemsByCatagory("all");

        itemLists[0] = cosmeticsHash.GetFreeItemsByCatagory("hat");
        itemLists[1] = cosmeticsHash.GetFreeItemsByCatagory("face");
        itemLists[2] = cosmeticsHash.GetFreeItemsByCatagory("badge");
        itemLists[3] = cosmeticsHash.GetFreeItemsByCatagory("chest");
        itemLists[4] = cosmeticsHash.GetFreeItemsByCatagory("holdable");

        currentWornSet.hat = cosmeticsHash.GetItemFromName(PlayerPrefs.GetString("hatCosmetic", "null"));
        currentWornSet.face = cosmeticsHash.GetItemFromName(PlayerPrefs.GetString("faceCosmetic", "null"));
        currentWornSet.badge = cosmeticsHash.GetItemFromName(PlayerPrefs.GetString("badgeCosmetic", "null"));
        currentWornSet.chest = cosmeticsHash.GetItemFromName(PlayerPrefs.GetString("chestCosmetic", "null"));
        currentWornSet.holdable = cosmeticsHash.GetItemFromName(PlayerPrefs.GetString("holdableCosmetic", "null"));
        
    }

    public void Start()
    {
        if(wardrobes.Count != 0)
        foreach (Wardrobe wd in wardrobes)
        {
            foreach (GameObject go in wd.CosmeticStands)
            {
                go.SetActive(true);
            }
            wd.sawPlayer = true;
            foreach (WardrobeFunctionButton wfb in wd.FunctionButtons)
            {
                wfb.SetCosmeticImage(currentWornSet);
            }
            
        }
        UpdateWardrobeModelsAndButtons();
        if(MiscGrabs.grab.DoesPlayerExist())
        RigController.Instance.LocalUpdateCosmetics(currentWornSet);
    }

    public void RefreshRigCosmetics()
    {
        if(MiscGrabs.grab.DoesPlayerExist())
        RigController.Instance.LocalUpdateCosmetics(currentWornSet);
    }

    public void PressCosmeticStandButton(CosmeticStand pressedStand)
    {
    }

    public void PressWardrobeItemButton(CosmeticItem cosmeticItem)
    {
        switch (cosmeticItem.itemSlot)
        {
            case "badge":
                currentWornSet.badge = (cosmeticItem.itemName == currentWornSet.badge.itemName) ? nullItem : cosmeticsHash.GetItemFromName(cosmeticItem.itemName);
                PlayerPrefs.SetString("badgeCosmetic", currentWornSet.badge.itemName);
                tryOnSet.badge = nullItem;
                break;
            case "face":
                currentWornSet.face = (cosmeticItem.itemName == currentWornSet.face.itemName) ? nullItem : cosmeticsHash.GetItemFromName(cosmeticItem.itemName);
                PlayerPrefs.SetString("faceCosmetic", currentWornSet.face.itemName);
                tryOnSet.face = nullItem;
                break;
            case "hat":
                currentWornSet.hat = (cosmeticItem.itemName == currentWornSet.hat.itemName) ? nullItem : cosmeticsHash.GetItemFromName(cosmeticItem.itemName);
                PlayerPrefs.SetString("hatCosmetic", currentWornSet.hat.itemName);
                tryOnSet.hat = nullItem;
                break;
            case "chest":
                currentWornSet.chest = (cosmeticItem.itemName == currentWornSet.chest.itemName) ? nullItem : cosmeticsHash.GetItemFromName(cosmeticItem.itemName);
                PlayerPrefs.SetString("chestCosmetic", currentWornSet.chest.itemName);
                tryOnSet.chest = nullItem;
                break;
            case "holdable":
                currentWornSet.holdable = (cosmeticItem.itemName == currentWornSet.holdable.itemName) ? nullItem : cosmeticsHash.GetItemFromName(cosmeticItem.itemName);
                PlayerPrefs.SetString("holdableCosmetic", currentWornSet.holdable.itemName);
                tryOnSet.holdable = nullItem;
                break;
            default:
                Debug.LogError("UNDEFINED COSMETIC ITEM SLOT, TYPE OF " + cosmeticItem.itemSlot);
                break;
        }
        PlayerPrefs.Save();
        UpdateWardrobeModelsAndButtons();
        RigController.Instance.LocalUpdateCosmetics(currentWornSet);
    }

    public void PressWardrobeFunctionButton(string function, WardrobeFunctionButton wfb)
    {
        switch (function)
        {
            case "chest":
                if (wardrobeType == 3)
                {
                    return;
                }
                wardrobeType = 3;
                break;
            case "badge":
                if (wardrobeType == 2)
                {
                    return;
                }
                wardrobeType = 2;
                break;
            case "face":
                if (wardrobeType == 1)
                {
                    return;
                }
                wardrobeType = 1;
                break;
            case "hat":
                if (wardrobeType == 0)
                {
                    return;
                }
                wardrobeType = 0;
                break;
            case "holdable":
                if (wardrobeType == 4)
                {
                    return;
                }
                wardrobeType = 4;
                break;
            case "right":
                cosmeticsPages[wardrobeType]++;
                if (cosmeticsPages[wardrobeType] > (itemLists[wardrobeType].Count - 1) / wfb.WD.CosmeticButtons.Count)
                {
                    cosmeticsPages[wardrobeType] = 0;
                }
                break;
            case "left":
                cosmeticsPages[wardrobeType]--;
                if (cosmeticsPages[wardrobeType] < 0)
                {
                    cosmeticsPages[wardrobeType] = (itemLists[wardrobeType].Count - 1) / wfb.WD.CosmeticButtons.Count;
                }
                break;
        }
        UpdateWardrobeModelsAndButtons();
    }

    public void UpdateWardrobeColorSliders(float redVal, float greenVal, float blueVal)
    {
        //Debug.Log("COSMETICS CONTROLLER : SETTING ALL WARDROBE COLOR SLIDERS AS; " + redVal + ", " + greenVal + ", " + blueVal);
        foreach (Wardrobe wd in wardrobes)
        {
            if (wd.IsType<SatelliteWardrobeController>())
            {
                SatelliteWardrobeController swc = wd.GetComponent<SatelliteWardrobeController>();
                swc.colorSliderController.SetSliderValues(redVal, greenVal, blueVal);
            }
        }
    }

    public void UpdateWardrobeModelsAndButtons()
    {
        foreach (Wardrobe WD in wardrobes)
        {
            foreach (WardrobeItemButton wib in WD.CosmeticButtons)
            {
                wib.currentCosmeticItem =
                (cosmeticsPages[wardrobeType] * WD.CosmeticButtons.Count + wib.cosmeticNum < itemLists[wardrobeType].Count) ? itemLists[wardrobeType][cosmeticsPages[wardrobeType] * WD.CosmeticButtons.Count + wib.cosmeticNum] : nullItem;

                wib.controlledModel.SetCosmeticActive(wib.currentCosmeticItem.displayName);
                
                wib.isOn = wib.currentCosmeticItem.itemName != "null" && AnyMatch(currentWornSet, wib.currentCosmeticItem);
                
                wib.UpdateColor();
            }
            foreach (WardrobeFunctionButton wfb in WD.FunctionButtons)
            {
                wfb.SetCosmeticImage(currentWornSet);
            }
            /*for (iterator = 0; iterator < WD.CosmeticButtons.Count; iterator++)
            {
                WD.CosmeticButtons[iterator].isOn = WD.CosmeticButtons[iterator].currentCosmeticItem.itemName != "null" && AnyMatch(currentWornSet, WD.CosmeticButtons[iterator].currentCosmeticItem);
                WD.CosmeticButtons[iterator].UpdateColor();
            }*/
            WD.SelfDoll.SetCosmeticActiveArray(SetToStringArray(currentWornSet));
        }
    }

    public bool AnyMatch(CosmeticSet set, CosmeticItem item)
    {
        if (set.hat.itemName == item.itemName || set.face.itemName == item.itemName || set.badge.itemName == item.itemName || set.chest.itemName == item.itemName || set.holdable.itemName == item.itemName)
        {
            return true;
        }

        return false;
    }

    public void Initialize()
    {
        if (base.gameObject.activeSelf)
        {
            GetUserCosmeticsAllowed();
        }
    }
    
    public void RecheckCosmeticsInventory()
    {
        GetUserCosmeticsAllowed();
        UpdateWardrobeModelsAndButtons();
    }

    public void GetUserCosmeticsAllowed()
    {
        
    }

    public string[] SetToStringArray(CosmeticSet set)
    {
        returnStringArray[0] = set.hat.displayName;
        returnStringArray[1] = set.face.displayName;
        returnStringArray[2] = set.badge.displayName;
        returnStringArray[3] = set.chest.displayName;
        returnStringArray[4] = set.holdable.displayName;
        return returnStringArray;
    }

    public string ListToString(List<CosmeticItem> list)
    {
        return string.Join(", ", list.ConvertAll(x => x.displayName).ToArray());
    }

    public void AddCosmeticStand(CosmeticStand cs)
    {
        if (!cosmeticStands.Contains(cs))
        {
            cosmeticStands.Add(cs);
        }
    }
}
}