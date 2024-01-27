using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CustomTrailRenderer : MonoBehaviour
{
    public float trailTime = 2.0f;
    public float minDistance = 0.1f;
    public Material trailMaterial;

    private Mesh trailMesh;
    private MeshFilter meshFilter;
    private List<Vector3> points;
    private List<float> times;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        trailMesh = new Mesh();
        meshFilter.mesh = trailMesh;
        points = new List<Vector3>();
        times = new List<float>();

        if (trailMaterial != null)
        {
            GetComponent<MeshRenderer>().material = trailMaterial;
        }
    }

    private void Update()
    {
        // Update points
        UpdatePoints();

        // Create mesh
        UpdateTrailMesh();
    }

    private void UpdatePoints()
    {
        if (points.Count == 0 || Vector3.Distance(transform.position, points[points.Count - 1]) > minDistance)
        {
            points.Add(transform.position);
            times.Add(Time.time);
        }

        // Remove old points
        for (int i = times.Count - 1; i >= 0; i--)
        {
            if (Time.time - times[i] > trailTime)
            {
                times.RemoveAt(i);
                points.RemoveAt(i);
            }
        }
    }

    private void UpdateTrailMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            // Add vertices for each segment
            Vector3 direction = (points[i + 1] - points[i]).normalized;
            Vector3 side = Vector3.Cross(direction, Vector3.up).normalized * 0.05f; // Width of the trail

            vertices.Add(points[i] + side);
            vertices.Add(points[i] - side);
            vertices.Add(points[i + 1] + side);
            vertices.Add(points[i + 1] - side);

            int index = i * 4;
            // Add triangles
            triangles.Add(index + 0);
            triangles.Add(index + 1);
            triangles.Add(index + 2);

            triangles.Add(index + 2);
            triangles.Add(index + 1);
            triangles.Add(index + 3);
        }

        trailMesh.Clear();
        trailMesh.SetVertices(vertices);
        trailMesh.SetTriangles(triangles, 0);
    }
}