using UnityEngine;
using System.Collections.Generic;

public class BoidsInstancedChase : MonoBehaviour
{
    [Header("Rendering")]
    public Mesh boidMesh;
    public Material boidMaterial;

    [Header("Boid Count")]
    [Range(1, 10000)]
    public int boidCount = 500;

    [Header("Movement")]
    public float maxSpeed = 5f;
    public float steeringForce = 5f;

    [Header("Boid Rules")]
    public float neighborRadius = 2.5f;
    public float separationRadius = 1f;
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 2f;

    [Header("Target")]
    public Transform target;
    public float targetAttractionWeight = 3f;

    struct Boid
    {
        public Vector3 position;
        public Vector3 velocity;
    }

    Boid[] boids;
    Matrix4x4[] matrices;

    const int BATCH_SIZE = 1023;

    void Start()
    {
        boids = new Boid[boidCount];
        matrices = new Matrix4x4[BATCH_SIZE];

        for (int i = 0; i < boidCount; i++)
        {
            boids[i].position = transform.position + Random.insideUnitSphere * 5f;
            boids[i].velocity = Random.onUnitSphere;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i < boids.Length; i++)
        {
            Vector3 pos = boids[i].position;

            Vector3 cohesion = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            Vector3 separation = Vector3.zero;
            int count = 0;

            for (int j = 0; j < boids.Length; j++)
            {
                if (i == j) continue;

                Vector3 toOther = boids[j].position - pos;
                float dist = toOther.magnitude;

                if (dist < neighborRadius)
                {
                    cohesion += boids[j].position;
                    alignment += boids[j].velocity;
                    count++;

                    if (dist < separationRadius)
                        separation -= toOther / dist;
                }
            }

            Vector3 accel = Vector3.zero;

            if (count > 0)
            {
                cohesion = ((cohesion / count) - pos).normalized;
                alignment = (alignment / count).normalized;
            }

            accel += cohesion * cohesionWeight;
            accel += alignment * alignmentWeight;
            accel += separation * separationWeight;

            if (target)
            {
                Vector3 toTarget = (target.position - pos).normalized;
                accel += toTarget * targetAttractionWeight;
            }

            boids[i].velocity += accel * steeringForce * dt;
            boids[i].velocity = Vector3.ClampMagnitude(boids[i].velocity, maxSpeed);
            boids[i].position += boids[i].velocity * dt;
        }

        DrawBoids();
    }

    void DrawBoids()
    {
        int index = 0;

        for (int i = 0; i < boids.Length; i++)
        {
            Quaternion rot = boids[i].velocity.sqrMagnitude > 0.0001f
                ? Quaternion.LookRotation(boids[i].velocity)
                : Quaternion.identity;

            matrices[index++] = Matrix4x4.TRS(
                boids[i].position,
                rot,
                Vector3.one
            );

            if (index == BATCH_SIZE)
            {
                Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, matrices, index);
                index = 0;
            }
        }

        if (index > 0)
            Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, matrices, index);
    }
}
