using System.Collections.Generic;
using UnityEngine;

public static class QuadMesh
{
    private static bool _cashEnabled;
    private static List<Mesh> _cache = new List<Mesh>();

    public static void EnableCache()
    {
        _cashEnabled = true;
    }

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
        /*
        if (_cashEnabled)
        {
            foreach (var item in _cache)
            {
                if (vertices[0] == item.vertices[0] &&
                vertices[1] == item.vertices[1] &&
                vertices[2] == item.vertices[2] &&
                vertices[3] == item.vertices[3])
                {
                    return item;
                }
            }
        }*/

        var mesh = new Mesh();

        mesh.vertices = vertices;

        var tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
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
        //mesh.Optimize();
        _cache.Add(mesh);

        return mesh;
    }
}