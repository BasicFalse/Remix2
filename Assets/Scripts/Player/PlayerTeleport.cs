using UnityEngine;

public class PlayerTeleport : PlayerMovementStyle
{
    public override void UpdatePlayer()
    {
        
    }

    public void DidTeleport()
    {
        @b.gravity.UpdateAlignment(instant: true);
        
    }
}
