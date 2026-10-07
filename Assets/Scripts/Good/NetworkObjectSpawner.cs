using Photon.Pun;
using UnityEngine;

public class NetworkObjectSpawner : MonoBehaviourPun
{
    public bool canSpawn;
    public bool spawnManual;
    [SerializeField] private string objectPrefabName; // Prefab name for Resources.Load
    [SerializeField] public Transform spawnPoint;    // Default spawn point
    private GameObject objectPrefab;

    private void Start()
    {
        // Load the prefab from Resources folder
        objectPrefabName = "CubeNetworkRigidbody";
        objectPrefab = Resources.Load<GameObject>(objectPrefabName);

        if (objectPrefab == null)
        {
            Debug.LogError($"[Error] Prefab '{objectPrefabName}' not found in Resources folder.");
        }
    }

    private void Update()
    {
        if (spawnManual)
        {
            spawnManual = false;
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(SpawnObjectRPC), RpcTarget.MasterClient, spawnPoint.position, spawnPoint.rotation.eulerAngles);
            }
            else
            {
                RequestSpawn(spawnPoint);
            }
        }
    }

    public void RequestSpawn(Transform spawnPos)
    {
        photonView.RPC(nameof(SpawnObjectRPC), RpcTarget.MasterClient, spawnPos.position, spawnPos.rotation.eulerAngles);
    }

    [PunRPC]
    void SpawnObjectRPC(Vector3 position, Vector3 eulerAngles)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[Client] Received SpawnObjectRPC but not the Master Client.");
            return;
        }

        if (objectPrefab != null && canSpawn)
        {
            Quaternion rotation = Quaternion.Euler(eulerAngles); // Convert Vector3 back to Quaternion
            PhotonNetwork.InstantiateRoomObject(objectPrefabName, position, rotation);
            Debug.Log($"[Master Client] Instantiated {objectPrefab.name} at {position}");
        }
        else
        {
            Debug.LogError("[Master Client] Prefab is not assigned or spawning is disabled.");
        }
    }

}
