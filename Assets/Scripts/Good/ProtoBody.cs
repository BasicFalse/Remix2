using Unity.Mathematics;
using UnityEngine;

public class ProtoBody : MonoBehaviour
{
    public enum ColliderType{Box,Sphere,Capsule}
    public ColliderType ColliderT;
    [Tooltip("Determines if the ProtoBody collides with trigger colliders, non-trigger colliders, or both.")]
    public bool2 CollisionType;

    [Tooltip("When the ProtoBody is hit, should it account for extra space?")]
    public float buffer;

    void OnCollisionEnter(Collision collision)
    {
        if(CollisionType.y) CollisionLogic(collision.body.gameObject.GetComponent<Collider>());
    }

    void OnTriggerEnter(Collider other)
    {
        if(CollisionType.x) CollisionLogic(other);
    }

    void CollisionLogic(Collider c)
    {
        if(c is BoxCollider)
        {
            BoxColliderLogic(c.GetComponent<BoxCollider>());
        }
        else if(c is SphereCollider)
        {
            SphereColliderLogic(c.GetComponent<SphereCollider>());
        }
    }

    void BoxColliderLogic(BoxCollider bc)
    {
        Debug.LogAssertion("Box collisions are not permitted.", this);
    }

    void SphereColliderLogic(SphereCollider sc)
    {
        if(ColliderT == ColliderType.Sphere)
        {
            Vector3 Distance3d = transform.position - sc.transform.position;
            Vector3 offset1 = Distance3d.normalized * sc.radius/2;
            Vector3 offset0 = Distance3d.normalized * buffer;
            Distance3d += offset0 + offset1;
            transform.position = Distance3d + sc.transform.position;
        }
    }
}
