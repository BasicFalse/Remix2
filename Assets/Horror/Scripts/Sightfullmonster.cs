using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem.Processors;

public class Sightfullmonster : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public float stoppingDistance = 1f;
    public float timeToShowGameObject = 2f;
    public GameObject gameobjectToShowOnCollision;
    public List<Transform> playerCameras;
    public List<Transform> spc;

    public Transform targetPlayer;
    public Transform targetPlayerCamera;
    public Vector3 initialPosition;
    public bool isCollided = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom)
        {
            float closestDistance = Mathf.Infinity;

            targetPlayer = spc[1];

            if (targetPlayer != null && closestDistance < chaseRange)
            {
                if (!IsPlayerLookingAtMonster(spc))
                {
                    // Move toward player
                    transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);

                    // Rotate toward player
                    Vector3 dir = targetPlayer.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }
            else
            {
                // Return to initial position
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);

                Vector3 dir = initialPosition - transform.position;
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        else
        {
            if (MiscGrabs.grab != null)
            {
                playerCameras = MiscGrabs.grab.PlayerCameras;
            }
            if (!PhotonNetwork.IsMasterClient) return;

            float closestDistance = Mathf.Infinity;
            Transform closestCamera = null;

            // Find the closest player and their camera
            foreach (Transform c in playerCameras)
            {
                float distance = Vector3.Distance(transform.position, c.transform.position);
                if (distance < closestDistance)
                {
                    Transform cam = c;
                    if (cam != null)
                    {
                        closestDistance = distance;
                        closestCamera = cam;
                    }
                }
            }

            targetPlayer = closestCamera.transform;
            targetPlayerCamera = closestCamera;

            if (targetPlayer != null && closestDistance < chaseRange)
            {
                if (!IsPlayerLookingAtMonster(playerCameras))
                {
                    // Move toward player
                    transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);

                    // Rotate toward player
                    Vector3 dir = targetPlayer.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }
            else
            {
                // Return to initial position
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);

                Vector3 dir = initialPosition - transform.position;
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        if (isCollided)
        {
            gameobjectToShowOnCollision.SetActive(true);
            Invoke("HideGameObject", timeToShowGameObject);
            isCollided = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (PhotonNetwork.IsMasterClient && collision.gameObject.CompareTag("GorillaPlayer"))
        {
            isCollided = true;
        }
    }

    void HideGameObject()
    {
        gameobjectToShowOnCollision.SetActive(false);
    }

    private bool IsPlayerLookingAtMonster(List<Transform> cams)
    {
        if (cams == null) return false;

        foreach (Transform c in cams)
        {
            if (c == null) continue;

            Vector3 toMonster = (transform.position - c.transform.position).normalized;
            float dot = Vector3.Dot(c.transform.forward, toMonster);
            if (dot > 0.75f) // Adjust threshold if needed
            {
                return true; // At least one player is looking
            }
        }

        return false; // No player is looking
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
