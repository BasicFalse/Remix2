using UnityEngine;
using Photon.Pun;
using UFunction;

public class DrunkenEnemyController : EnemyController
{
    protected override void MoveAndRotateToward(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;

        direction += new Vector3(Random.Range(0,1)*direction.magnitude/2,Random.Range(0,1)*direction.magnitude/2,Random.Range(0,1)*direction.magnitude/2);

        transform.LookAt(direction);

        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }
}
