using UnityEngine;
using GorillaRigging;

[RequireComponent(typeof(SparkleDirector))]
public class SparkleSwarm : MonoBehaviour
{
    public Vector3 averagePosition;

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
    public float targetAttractionWeight = 3f;
    public Transform target;

    [Header("Random Spin & Size")]
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    public float minSpinSpeed = -180f;
    public float maxSpinSpeed = 180f;

    struct Boid
    {
        public Vector3 position;
        public Vector3 velocity;
        public float size;
        public float spinSpeed;
        public float spinAngle;
        public float hueOffset;
    }

    Boid[] boids;
    Matrix4x4[] matrices;

    MaterialPropertyBlock mpb;
    float[] hueOffsets;

    const int BATCH_SIZE = 1023;

    void Start()
    {
        boids = new Boid[boidCount];
        matrices = new Matrix4x4[BATCH_SIZE];
        hueOffsets = new float[BATCH_SIZE];
        mpb = new MaterialPropertyBlock();

        for (int i = 0; i < boidCount; i++)
        {
            boids[i].position = transform.position + Random.insideUnitSphere * 5f;
            boids[i].velocity = Random.onUnitSphere;

            boids[i].size = Random.Range(minSize, maxSize);
            boids[i].spinSpeed = Random.Range(minSpinSpeed, maxSpinSpeed);
            boids[i].spinAngle = Random.Range(0f, 360f);
            boids[i].hueOffset = Random.value;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 centerSum = Vector3.zero;


        for (int i = 0; i < boids.Length; i++)
        {
            Vector3 pos = boids[i].position;
            centerSum += boids[i].position;


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

            boids[i].spinAngle += boids[i].spinSpeed * dt;
        }
        averagePosition = centerSum / boids.Length;

        DrawBoids();
        EvaluateDistance();
    }

    void DrawBoids()
    {
        int index = 0;
    
        for (int i = 0; i < boids.Length; i++)
        {
            // Only use spin, no velocity-based rotation
            Quaternion spinRot = Quaternion.AngleAxis(boids[i].spinAngle, Vector3.forward);
    
            matrices[index] = Matrix4x4.TRS(
                boids[i].position,
                spinRot, // no LookRotation anymore
                Vector3.one * boids[i].size
            );
    
            hueOffsets[index] = boids[i].hueOffset;
    
            index++;
    
            if (index == BATCH_SIZE)
            {
                mpb.SetFloatArray("_InstanceOffset", hueOffsets);
                Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, matrices, index, mpb);
                index = 0;
            }
        }
    
        if (index > 0)
        {
            mpb.SetFloatArray("_InstanceOffset", hueOffsets);
            Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, matrices, index, mpb);
        }
    }

    void EvaluateDistance()
    {
        float minDistance = 888;
        float lastboidDistance;
        if(MiscGrabs.grab.DoesPlayerExist() == false)
        for(int i =0; i < boids.Length; i++)
        {
            lastboidDistance = Vector3.Distance(boids[i].position, MiscGrabs.grab.GetPlayerPosition());
            if(lastboidDistance < minDistance)
            {
                minDistance = lastboidDistance;
            }
            
        }
    }

    public void SetMood(SparkleBehaviorContainer SBC)
    {
        maxSpeed = SBC.maxSpeed;
        steeringForce = SBC.steeringForce;
        neighborRadius = SBC.neighborRadius;
        separationRadius = SBC.separationRadius;
        cohesionWeight = SBC.cohesionWeight;
        alignmentWeight = SBC.alignmentWeight;
        separationWeight = SBC.separationWeight;
        targetAttractionWeight = SBC.targetAttractionWeight;
    }

}
