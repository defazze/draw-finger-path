using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public static class MeshHelper
{
    public static Mesh CreateQuad()
    {
        var vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };

        return CreateQuad(vertices);
    }

    public static Mesh CreateQuad(Vector3[] vertices)
    {
        var mesh = new Mesh();

        mesh.vertices = vertices;

        var tris = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };

        mesh.triangles = tris;

        var normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        var uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        return mesh;
    }
    public static Mesh CreateQuad(Bounds bounds)
    {
        var vertex0 = Vector3.zero - bounds.extents;
        var vertex1 = new Vector3(bounds.extents.x, -1 * bounds.extents.y, 0);
        var vertex2 = new Vector3(-1 * bounds.extents.x, bounds.extents.y, 0);
        var vertex3 = bounds.extents;

        var mesh = CreateQuad(new[] { vertex0, vertex1, vertex2, vertex3 });
        return mesh;
    }

    public static Mesh CreateCircle(float radius)
    {
        const int n = 20;
        var mesh = new Mesh();

        List<Vector3> verticiesList = new List<Vector3> { };
        float x;
        float y;
        for (int i = 0; i < n; i++)
        {
            x = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
            y = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
            verticiesList.Add(new Vector3(x, y, 0f));
        }
        Vector3[] verticies = verticiesList.ToArray();

        //triangles
        List<int> trianglesList = new List<int> { };
        for (int i = 0; i < (n - 2); i++)
        {
            trianglesList.Add(0);
            trianglesList.Add(i + 1);
            trianglesList.Add(i + 2);
        }
        int[] triangles = trianglesList.ToArray();

        //normals
        List<Vector3> normalsList = new List<Vector3> { };
        for (int i = 0; i < verticies.Length; i++)
        {
            normalsList.Add(-Vector3.forward);
        }
        Vector3[] normals = normalsList.ToArray();

        //initialise
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

    public static Mesh Take(this Mesh originalMesh, int startQuad = 0, int offset = 1)
    {
        var result = new Mesh();

        var startVertice = startQuad * 4;
        var verticesCount = offset * 4;

        result.vertices = originalMesh.vertices.Skip(startVertice).Take(verticesCount).ToArray();
        result.triangles = originalMesh.triangles.Skip(startVertice / 2 * 3).Take(verticesCount / 2 * 3).Select(t => t - startVertice).ToArray();
        result.normals = originalMesh.normals.Skip(startVertice).Take(offset * 4).ToArray();
        result.uv = originalMesh.uv.Skip(startVertice).Take(verticesCount).ToArray();

        return result;
    }

    public static IEnumerable<int3> GetTriangles(this Mesh mesh)
    {
        var triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            yield return new int3(triangles[i], triangles[i + 1], triangles[i + 2]);
        }

    }

    public static BlobAssetReference<Unity.Physics.Collider> CreateCollider(this Mesh mesh)
    {
        var vertices = new NativeArray<float3>(mesh.vertices.Select(v => (float3)v).ToArray(), Allocator.Temp);
        var triangles = new NativeArray<int3>(mesh.GetTriangles().ToArray(), Allocator.Temp);

        return Unity.Physics.MeshCollider.Create(vertices, triangles);
    }
}