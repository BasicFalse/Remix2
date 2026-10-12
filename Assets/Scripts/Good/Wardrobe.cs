using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using GorillaNetworking;

public class Wardrobe : RemixScript
{
    public HeadModel SelfDoll;

    public List<WardrobeItemButton> CosmeticButtons;

    public List<WardrobeFunctionButton> FunctionButtons;

    public List<GameObject> CosmeticStands;

    private GameObject gorillaPlayer;
    public bool sawPlayer;
    public bool sawSawPlayer;
    public float currentDistance;
    public float sightDistance;

    void Start()
    {
        if (!CosmeticsController.instance.wardrobes.Contains(this))
        {
            CosmeticsController.instance.wardrobes.Add(this);
            CosmeticsController.instance.UpdateWardrobeModelsAndButtons();
        }

    }

    void Update()
    {
        if(!MiscGrabs.grab.DoesPlayerExist()) return;
        if (gorillaPlayer == null) { gorillaPlayer = PlayerBase.@i.gameObject; }

        currentDistance = Vector3.Distance(transform.position, gorillaPlayer.transform.position);

        if (sawPlayer != sawSawPlayer)
        {
            sawSawPlayer = sawPlayer;
            if (sawPlayer == true)
            {
                CosmeticsController.instance.UpdateWardrobeModelsAndButtons();
            }
        }

        if (currentDistance <= sightDistance)
        {
            foreach (GameObject go in CosmeticStands)
            {
                go.SetActive(true);
            }
            CosmeticsController.instance.UpdateWardrobeModelsAndButtons();
            sawPlayer = true;
        }
        else
        {
            foreach (GameObject go in CosmeticStands)
            {
                go.SetActive(false);
            }
            sawPlayer = false;
        }
        
        
    }

    public virtual void OnEnable()
    {
        if(CosmeticsController.instance){
        CosmeticsController.instance.UpdateWardrobeModelsAndButtons();}
    }
}
