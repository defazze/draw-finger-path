using System.Linq;
using UnityEngine;

public static class QuadMesh
{
    public static Mesh Create()
    {
        var vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };

        return Create(vertices);
    }

    public static Mesh Create(Vector3[] vertices)
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
}