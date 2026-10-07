using GorillaNetworking;

public class SatelliteWardrobeController : Wardrobe
{
    public WardrobeColorController colorSliderController;

    public override void OnEnable()
    {
        base.OnEnable();
        if(GorillaComputer.instance != null){GorillaComputer.instance.RefreshWardrobeSliders();}
    }
}
