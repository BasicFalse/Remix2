using GorillaLocomotion;
using UnityEngine;

public class Jumpscare2Point2 : MonoBehaviour
{
    public JumpScareManager.JumpscareSetting Setting;
    public JumpScareManager.JumpscareType JST;
    public float jumpscareDuration;
    public Gradient ColorLayout;
    public float lerpValue;

    public bool isJumpscaring = false;

    private void Awake()
    {
        if(jumpscareDuration <= 0){ jumpscareDuration = 2f; }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GorillaPlayer"))
        {
            if (!isJumpscaring)
            {
                PlayerBase player = PlayerBase.@i;
                player.bools.isGettingJumped = true;
                isJumpscaring = true;
                switch (Setting)
                {
                    case JumpScareManager.JumpscareSetting.Classic:
                        JumpScareManager.jump.CallJumpscare(JST, jumpscareDuration, this);
                        break;

                    case JumpScareManager.JumpscareSetting.Gradient:
                        JumpScareManager.jump.CallCustomJumpscare(ColorLayout,  lerpValue);
                        break;
                }
                
            }
        }
    }
    
}
