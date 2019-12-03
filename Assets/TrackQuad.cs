using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TrackQuad : IComponentData
{
    public float3 vertice0;
    public float3 vertice1;
    public float3 vertice2;
    public float3 vertice3;

    public Entity track;

    public void Set(Vector3[] vertices)
    {
        vertice0 = vertices[0];
        vertice1 = vertices[1];
        vertice2 = vertices[2];
        vertice3 = vertices[3];
    }


    public void Set(Vector3[] vertices, Vector3 locate)
    {
        vertice0 = vertices[0] + locate;
        vertice1 = vertices[1] + locate;
        vertice2 = vertices[2] + locate;
        vertice3 = vertices[3] + locate;
    }

    public Vector3[] Get()
    {
        return new[] { (Vector3)vertice0, (Vector3)vertice1, (Vector3)vertice2, (Vector3)vertice3 };
    }
}