using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Triangle : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertexBuffer;
    private List<int> triangleBuffer;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Triangle Mesh";

        vertexBuffer = new List<Vector3> { Vector3.zero, Vector3.zero, Vector3.zero };
        triangleBuffer = new List<int> { 0, 2, 1 };
    }

    public void SetVertex(int index, Vector3 position)
    {
        if (index < 0 || index > 2)
        {
            return;
        }

        if (vertexBuffer.Count > index)
        {
            vertexBuffer[index] = position;
        }

        mesh.Clear();
        mesh.SetVertices(vertexBuffer);
        mesh.SetTriangles(triangleBuffer, 0);
        mesh.RecalculateNormals();
        mesh.Optimize();
    }

    public void Clear()
    {
        mesh.Clear();
    }
}
