using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChartivarController : MonsterBase, IPunObservable
{
    public enum ChartivarState { seeking, waiting, psst, scaring }

    public ChartivarState currentState;
    public bool isScaring;
    public float psstTimer;

    public AudioClip psstSound;
    public AudioClip jumpscareSound;

    public AudioSource CASM;

    public MeshRenderer ChartivarFace;
    public Material whiteEyes;
    public Material scaringFace;
    public GameObject teeth;
    public GameObject ChartivarMesh;

    public Transform Player;

    public LayerMask obstructionMask;

    public List<Transform> spawns;


    protected override void Start()
    {
        ChartivarMesh.SetActive(false);
        ChartivarFace.material = whiteEyes;
        teeth.SetActive(false);

        currentState = ChartivarState.seeking;
        base.Start();
    }

    public override void RegardlessMovement()
    {
        switch (currentState)
        {
            case ChartivarState.seeking:
                SeekingState();
                break;

            case ChartivarState.waiting:
                WaitingState();
                break;

            case ChartivarState.psst:
                PsstState();
                break;

            case ChartivarState.scaring:
                ScareState();
                break;

            default:
                currentState = ChartivarState.seeking;
                break;
        }
    }


    public void SeekingState()
    {
        Transform closestSpawn = null;
        float minDistance = 30f;

        foreach (Transform spawn in spawns)
        {
            float dist = Vector3.Distance(spawn.position, Player.position);

            if (dist < minDistance)
            {
                Vector3 dirToPlayer = (Player.position - spawn.position).normalized;
                float rayDist = dist;

                if (!Physics.Raycast(spawn.position, dirToPlayer, rayDist, obstructionMask))
                {
                    minDistance = dist;
                    closestSpawn = spawn;
                }
            }
        }

        if (closestSpawn != null)
        {
            ChartivarMesh.transform.SetPositionAndRotation(closestSpawn.position, closestSpawn.rotation);

            currentState = ChartivarState.waiting;
        }
    }

    public void WaitingState()
    {
        Vector3 toChartivar = (ChartivarMesh.transform.position - Player.position).normalized;
        float dot = Vector3.Dot(Player.forward, toChartivar);

        if (dot < 0.4f)
        {
            ChartivarMesh.SetActive(true);
            currentState = ChartivarState.psst;
        }
    }


    public void PsstState()
    {
        if (psstTimer <= 0f)
        {
            psstTimer = 10f;
            CASM.PlayOneShot(psstSound);
        }
        else { psstTimer -= 1.5f * Time.deltaTime; }

        teeth.SetActive(false);
        ChartivarFace.material = whiteEyes;
        ChartivarMesh.SetActive(true);


        Vector3 toChartivar = (ChartivarMesh.transform.position - Player.position).normalized;
        float dot = Vector3.Dot(Player.forward, toChartivar);

        if (dot >= 0.7f && DistanceToClosestPlayer(ChartivarMesh.transform.position) < 7f)
        {
            currentState = ChartivarState.scaring;
        }
        if(dot <= 0.4 && DistanceToClosestPlayer(ChartivarMesh.transform.position) > 15f)
        {
            ChartivarMesh.SetActive(false);
            currentState = ChartivarState.seeking;
        }
    }

    public void ScareState()
    {
        Debug.Log("Chartivar Scare State Initial");
        if (!isScaring)
        {
            Debug.Log("Chartivar Scare State Triggered");
            InitiateScare();
        }
    }

    public void InitiateScare()
    {
        ChartivarFace.material = scaringFace;
        teeth.SetActive(true);
        isScaring = true;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(StateToInt(currentState));
            stream.SendNext(isScaring);
            stream.SendNext(psstTimer);
        }
        else
        {
            currentState = IntToState((int)stream.ReceiveNext());
            isScaring = (bool)stream.ReceiveNext();
            psstTimer = (float)stream.ReceiveNext();
        }
    }

    public ChartivarState IntToState(int i)
    {
        return i switch
        {
            0 => ChartivarState.seeking,
            1 => ChartivarState.waiting,
            2 => ChartivarState.psst,
            3 => ChartivarState.scaring,
            _ => ChartivarState.seeking,
        };
    }
    public int StateToInt(ChartivarState s)
    {
        return s switch
        {
            ChartivarState.seeking => 0,
            ChartivarState.waiting => 1,
            ChartivarState.psst => 2,
            ChartivarState.scaring => 3,
            _ => 0,
        };
    }
    public void OnDrawGizmos()
    {
        Vector3 toChartivar = (ChartivarMesh.transform.position - Player.position).normalized;
        Gizmos.DrawLine(Player.position, toChartivar);
    }
}
