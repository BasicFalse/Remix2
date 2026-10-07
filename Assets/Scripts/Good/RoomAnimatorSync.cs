using UnityEngine;
using Photon.Pun;

public class RoomAnimatorSync : MonoBehaviourPun
{
    public Animator animator;

    // Received from network
    private int[] targetHashes;
    private float[] targetTimes;

    // Interpolated local values
    private float[] currentTimes;

    // How fast to interpolate (10–20 recommended)
    public float interpSpeed = 12f;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        int layerCount = animator.layerCount;

        targetHashes = new int[layerCount];
        targetTimes = new float[layerCount];
        currentTimes = new float[layerCount];

        // Initialize time arrays
        for (int i = 0; i < layerCount; i++)
        {
            targetHashes[i] = 0;
            targetTimes[i] = 0f;
            currentTimes[i] = 0f;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
            return;

        // Remote clients: interpolate
        for (int i = 0; i < animator.layerCount; i++)
        {
            AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(i);

            bool stateChanged = (st.fullPathHash != targetHashes[i]);

            if (stateChanged)
            {
                // Snap directly into the correct state & starting time
                animator.Play(targetHashes[i], i, targetTimes[i]);
                currentTimes[i] = targetTimes[i];
            }
            else
            {
                // Timeline interpolation
                currentTimes[i] = Mathf.Lerp(
                    currentTimes[i],
                    targetTimes[i],
                    Time.deltaTime * interpSpeed
                );

                // Apply interpolated time
                animator.Play(targetHashes[i], i, currentTimes[i]);
            }
        }
    }

    void LateUpdate()
    {
        if (!photonView.IsMine)
            return;

        int layerCount = animator.layerCount;
        int[] hashes = new int[layerCount];
        float[] times = new float[layerCount];

        for (int i = 0; i < layerCount; i++)
        {
            AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(i);

            hashes[i] = st.fullPathHash;

            // FIX: always clamp normalizedTime to 0–1 range
            times[i] = st.normalizedTime % 1f;
        }

        if(PhotonNetwork.InRoom){
        photonView.RPC("RPC_SyncAnimator", RpcTarget.Others, hashes, times);}
    }

    [PunRPC]
    private void RPC_SyncAnimator(int[] hashes, float[] times)
    {
        for (int i = 0; i < animator.layerCount; i++)
        {
            targetHashes[i] = hashes[i];
            targetTimes[i] = times[i];
        }
    }
}
