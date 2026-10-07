using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class AdvancedRaycastPathLine : MonoBehaviour
{
    public Transform target;
    public LayerMask obstacleMask;

    [Header("Grid Settings")]
    public float nodeSpacing = 1f;
    public int gridRadius = 10; // nodes in each direction

    [Header("Path Settings")]
    public int maxIterations = 500;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
    }

    void Update()
    {
        if (!target) return;

        List<Vector3> path = FindPath(transform.position, target.position);
        DrawPath(path);
    }

    #region Pathfinding

    class Node
    {
        public Vector3 pos;
        public float g, h;
        public Node parent;
        public float f => g + h;
    }

    List<Vector3> FindPath(Vector3 start, Vector3 goal)
    {
        Node startNode = new() { pos = start };
        Node goalNode = new() { pos = goal };

        List<Node> open = new() { startNode };
        HashSet<Vector3> closed = new();

        int iterations = 0;

        while (open.Count > 0 && iterations++ < maxIterations)
        {
            open.Sort((a, b) => a.f.CompareTo(b.f));
            Node current = open[0];
            open.RemoveAt(0);

            if (Vector3.Distance(current.pos, goal) < nodeSpacing)
                return ReconstructPath(current, goal);

            closed.Add(current.pos);

            foreach (Vector3 neighborPos in GetNeighbors(current.pos))
            {
                if (closed.Contains(neighborPos))
                    continue;

                // Blocked edge?
                if (Physics.Raycast(current.pos, neighborPos - current.pos,
                    Vector3.Distance(current.pos, neighborPos), obstacleMask))
                    continue;

                float tentativeG = current.g + Vector3.Distance(current.pos, neighborPos);

                Node neighbor = open.Find(n => n.pos == neighborPos);
                if (neighbor == null)
                {
                    neighbor = new Node
                    {
                        pos = neighborPos,
                        g = tentativeG,
                        h = Vector3.Distance(neighborPos, goal),
                        parent = current
                    };
                    open.Add(neighbor);
                }
                else if (tentativeG < neighbor.g)
                {
                    neighbor.g = tentativeG;
                    neighbor.parent = current;
                }
            }
        }

        return new List<Vector3>(); // no path
    }

    List<Vector3> GetNeighbors(Vector3 pos)
    {
        List<Vector3> neighbors = new();

        for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        for (int z = -1; z <= 1; z++)
        {
            if (x == 0 && y == 0 && z == 0)
                continue;

            Vector3 offset = new Vector3(x, y, z) * nodeSpacing;
            Vector3 candidate = pos + offset;

            if (Vector3.Distance(candidate, transform.position) <= gridRadius * nodeSpacing)
                neighbors.Add(candidate);
        }

        return neighbors;
    }

    List<Vector3> ReconstructPath(Node endNode, Vector3 finalGoal)
    {
        List<Vector3> path = new();
        Node current = endNode;

        while (current != null)
        {
            path.Add(current.pos);
            current = current.parent;
        }

        path.Reverse();
        path.Add(finalGoal);
        return path;
    }

    #endregion

    void DrawPath(List<Vector3> path)
    {
        if (path == null || path.Count == 0)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount = path.Count;
        line.SetPositions(path.ToArray());
    }
}
