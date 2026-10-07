using GorillaLocomotion;
using UnityEngine;
using GravNew;
using System.Collections;
using Photon.Pun;
using GorillaNetworking;
using GorillaRigging;

public enum FlashEffect
{
    Empty,
    Flashbang,
    Blackbang,
    Stun,
    Slow,
    Disorient
}
public class PlayerLocalStatusKeeper : MonoBehaviourPun
{
#pragma warning disable UDR0001 // Domain Reload Analyzer
    public static PlayerLocalStatusKeeper Instance { get; private set; }
#pragma warning restore UDR0001 // Domain Reload Analyzer
    public Rigidbody RB;
    public PlayerBase PB;
    public NormalisedGravity NG;
    public GorillaClimb RC;
    public GorillaClimb LC;

    private int CurrentScreenEffectIndex=0;
    private bool lockRotation = true;
    private bool reorient = true;
    private bool LHClimb;
    private bool RHClimb;

    public MeshRenderer ScreenColorOverlay;

    public Gradient FlashbangGradient;
    public Gradient BlackbangGradient;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        NG.reorient = reorient;
        RB.freezeRotation = lockRotation;
        Color final = new(){r = 0,g=0,b=0,a=0};
        ScreenColorOverlay.sharedMaterial.SetColor("_EmissionColor", final);
        ScreenColorOverlay.sharedMaterial.SetColor("_BaseColor", final);
    }

    void Update()
    {
        if(LC.climbing != LHClimb)
        {
            reorient = !LC.climbing;
            LHClimb = LC.climbing;
        }
        if(RC.climbing != RHClimb)
        {
            reorient = !RC.climbing;
            RHClimb = RC.climbing;
        }
        NG.reorient = reorient;
        RB.freezeRotation = lockRotation;
    }

    public void RequestEffect(FlashEffect type, float duration, Vector3 position, bool networked)
    {
        switch (type)
        {
            case FlashEffect.Flashbang:
                FlashLikeEffect(FlashbangGradient, duration, position);
                break;
            case FlashEffect.Blackbang:
                FlashLikeEffect(BlackbangGradient, duration, position);
                break;
            case FlashEffect.Disorient:
                StartCoroutine(Disorient(duration));
                break;
        }
        

        if (networked && GorillaComputer.instance.isInRoom)
        {
            photonView.RPC("SendFlashEffectCall", RpcTarget.Others, FlashEffectToInt(type), duration, position);
        }
    }

    [PunRPC]
    void SendFlashEffectCall(int type, float duration, Vector3 position)
    {
        RequestEffect(IntToFlashEffect(type), duration, position, false);
    }

    void FlashLikeEffect(Gradient gradient, float duration, Vector3 position)
    {
        StartCoroutine(GradientFlash(gradient, ScreenColorOverlay, duration, ++CurrentScreenEffectIndex));
    }


    IEnumerator Disorient(float duration)
    {
        float elapsed = 0f;
        reorient = false;
        lockRotation = false;
        yield return null;
        RB.AddTorque(new Vector3(Random.Range(-1000,1000),Random.Range(-1000,1000),Random.Range(-1000,1000)));

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            reorient = false;
            lockRotation = false;
            yield return null;
        }


        reorient = true;
        lockRotation = true;
    }
    IEnumerator GradientFlash(Gradient gradient, MeshRenderer mr, float duration, int FlashEffectIndex)
    {
        float elapsed = 0f;

        while (elapsed < duration && CurrentScreenEffectIndex == FlashEffectIndex)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Color color = gradient.Evaluate(t);
            mr.sharedMaterial.SetColor("_EmissionColor", color);
            mr.sharedMaterial.SetColor("_BaseColor", color);

            yield return null;
        }

        if(CurrentScreenEffectIndex == FlashEffectIndex){
        Color final = gradient.Evaluate(1f);
        mr.sharedMaterial.SetColor("_EmissionColor", final);
        mr.sharedMaterial.SetColor("_BaseColor", final);
        }
    }

    static int FlashEffectToInt(FlashEffect effect)
    {
        return effect switch
        {
            FlashEffect.Flashbang => 0,
            FlashEffect.Blackbang => 1,
            FlashEffect.Stun => 2,
            FlashEffect.Slow => 3,
            FlashEffect.Disorient => 4,
            _ => -1,
        };
    }

    static FlashEffect IntToFlashEffect(int Integer)
    {
        return Integer switch
        {
            0 => FlashEffect.Flashbang,
            1 => FlashEffect.Blackbang,
            2 => FlashEffect.Stun,
            3 => FlashEffect.Slow,
            4 => FlashEffect.Disorient,
            _ => FlashEffect.Empty,
        };
    }
}
