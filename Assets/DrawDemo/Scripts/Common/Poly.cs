using Unity.Burst;
using UnityEngine;

[BurstCompile]
public static class Poly
{
    public static bool ContainsPoint(Vector3[] vertices, Vector3 p, out int[] quadIndexes)
    {
        quadIndexes = new int[0];
        for (int i = 0; i < vertices.Length; i += 4)
        {
            var quad = new[] { vertices[i], vertices[i + 1], vertices[i + 2], vertices[i + 3] };
            if (IsInPolygon(quad, p))
            {
                quadIndexes = new[] { i, i + 1, i + 2, i + 3 };
                return true;
            }
        }

        return false;
    }

    private static bool IsInPolygon(Vector3[] vertices, Vector3 p)
    {
        var length = vertices.Length;
        var ordered = new[] { vertices[1], vertices[0], vertices[2], vertices[3] };

        bool isInPolygon = false;
        var lastVertex = ordered[length - 1];
        foreach (var vertex in ordered)
        {
            if (p.y.IsBetween(lastVertex.y, vertex.y))
            {
                double t = (p.y - lastVertex.y) / (vertex.y - lastVertex.y);
                double x = t * (vertex.x - lastVertex.x) + lastVertex.x;
                if (x >= p.x) isInPolygon = !isInPolygon;
            }
            else
            {
                if (p.y == lastVertex.y && p.x < lastVertex.x && vertex.y > p.y) isInPolygon = !isInPolygon;
                if (p.y == vertex.y && p.x < vertex.x && lastVertex.y > p.y) isInPolygon = !isInPolygon;
            }

            lastVertex = vertex;
        }

        return isInPolygon;
    }

    private static bool IsBetween(this float x, float a, float b)
    {
        return (x - a) * (x - b) < 0;
    }
}