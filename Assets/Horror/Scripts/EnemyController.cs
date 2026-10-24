using UnityEngine;
using Photon.Pun;
using UFunction;

public class EnemyController : MonsterBase
{
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public float stoppingDistance = 1f;
    public float timeToShowGameObject = 2f;
    public float rotationSensitivity = 2f;

    public Vector3 targetPlayer;
    public bool isCollided = false;

    public override void UpdateMonster()
    {
        if(OutOfBounds())
        {
            transform.SetPositionAndRotation(initialPosition, Quaternion.identity);
        }

        base.UpdateMonster();
    }

    public override void OfflineMovement()
    {
        if(!MiscGrabs.grab.DoesPlayerExist()){
            MoveAndRotateToward(initialPosition);
            return;
        }
        targetPlayer = GetPlayerPosition();

        float currentDistance = Vector3.Distance(transform.position, targetPlayer);
        if (currentDistance < chaseRange)
        {
            MoveAndRotateToward(targetPlayer);
        }
        else
        {
            MoveAndRotateToward(initialPosition);
        }
    }

    public override void OnlineMasterMovement()
    {
        if(moveSpeed < 1f || moveSpeed > 20f)
        {
            Debug.Log("Enemy " + gameObject.name + " has invalid move speed: " + moveSpeed, this);
            Startup.CallIt();
        }

        if(chaseRange < 5f || chaseRange > 30f)
        {
            Debug.Log("Enemy " + gameObject.name + " has invalid chase range: " + chaseRange, this);
            Startup.CallIt();
        }
        
        targetPlayer = ClosestPlayer();

        if (targetPlayer != null)
        {
            float currentDistance = Vector3.Distance(transform.position, targetPlayer);
            if (currentDistance < chaseRange)
            {
                MoveAndRotateToward(targetPlayer);
            }
            else
            {
                MoveAndRotateToward(initialPosition);
            }
        }
        else
        {
            MoveAndRotateToward(initialPosition);
        }
        if (isCollided)
        {
            Invoke("HideGameObject", timeToShowGameObject);
            isCollided = false;
        }
    }

    public override void OnlineSlaveMovement()
    {
        // brrt
    }


    public bool OutOfBounds()
    {
        bool oob = false;
        if(transform.position.x > 1000f || transform.position.y > 100f || transform.position.z > 1000f)
        {
            oob = true;
        }
        if(transform.position.x < -1000f || transform.position.y < -100f || transform.position.z < -1000f)
        {
            oob = true;
        }
        return oob;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if(init && resetPosition){transform.SetPositionAndRotation(initialPosition, Quaternion.identity);}
    }

    protected virtual void MoveAndRotateToward(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            float rotationSpeed = moveSpeed * rotationSensitivity;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GorillaPlayer") && PhotonNetwork.IsMasterClient)
        {
            isCollided = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
