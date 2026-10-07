using UnityEngine;
using UnityEngine.AI;

public class MonFollow : MonsterBase
{
    public NavMeshAgent Monster;
    public string playerTag = "Player"; // Customizable player tag in the editor
    [SerializeField] private float followRadius = 10.0f; // Radius within which the monster will follow the player
    [SerializeField] private float wanderRadius = 20.0f; // Radius within which the monster will wander
    [SerializeField] private float wanderTimer = 5.0f; // Time between wander updates
    [SerializeField] private float destinationReachedThreshold = 1.0f; // Distance threshold to consider the destination reached

    private float timer;
    private Vector3 wanderDestination;

    // Start is called before the first frame update
    new void Start()
    {
        timer = wanderTimer;
        PickNewWanderDestination();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(Monster.transform.position, player.transform.position);

            if (distanceToPlayer <= followRadius)
            {
                Monster.SetDestination(player.transform.position);
            }
            else
            {
                Wander();
            }
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer || Vector3.Distance(Monster.transform.position, wanderDestination) <= destinationReachedThreshold)
        {
            PickNewWanderDestination();
            timer = 0;
        }

        Monster.SetDestination(wanderDestination);
    }

    void PickNewWanderDestination()
    {
        wanderDestination = RandomNavSphere(Monster.transform.position, wanderRadius, -1);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    // Draw the follow radius and wander radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}