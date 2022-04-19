using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Sector : MonoBehaviour
{
    public int maxNumVertices = 60;

    private Mesh mesh;
    private List<Vector3> vertexBuffer;
    private List<int> triangleBuffer;
    private int currentVertexIndex;

    public int PositionCount => currentVertexIndex;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Sector Mesh";

        vertexBuffer = new List<Vector3>(maxNumVertices + 1);
        triangleBuffer = new List<int>(3 * maxNumVertices);
        currentVertexIndex = 0;
    }

    public void AddVertex(Vector3 vertex)
    {
        if (currentVertexIndex >= maxNumVertices)
        {
            return;
        }

        // Add the vertex
        vertexBuffer.Add(vertex);

        // Add the triangle
        if (currentVertexIndex >= 2)
        {
            triangleBuffer.Add(0);
            triangleBuffer.Add(currentVertexIndex);
            triangleBuffer.Add(currentVertexIndex - 1);
        }

        // Increment the vertex index
        currentVertexIndex++;

        mesh.Clear();
        mesh.SetVertices(vertexBuffer);
        mesh.SetTriangles(triangleBuffer, 0);

        mesh.RecalculateNormals();
        mesh.Optimize();
    }

    public void CloseSector()
    {
        AddVertex(vertexBuffer[1]);
    }

    public void Clear()
    {
        mesh.Clear();
        vertexBuffer.Clear();
        triangleBuffer.Clear();
        currentVertexIndex = 0;
    }

    public Vector3 GetLastPosition()
    {
        return vertexBuffer[vertexBuffer.Count - 1];
    }
}
