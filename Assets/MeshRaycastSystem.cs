using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class MeshRaycastSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (GameManager.Instanse.eraseMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                point = new Vector3 { x = point.x, y = point.y, z = 0 };

                Entities.WithAll<Track>().ForEach((Entity e, RenderMesh render) =>
                {
                    var mesh = render.mesh;
                    var inside = Poly.IsInPolygon(mesh.vertices, point);

                    if (inside)
                    {
                        PostUpdateCommands.DestroyEntity(e);
                    }
                });
            }
        }
    }
}

public static class Poly
{
    public static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
    {
        var j = polyPoints.Length - 1;
        var inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            var pi = polyPoints[i];
            var pj = polyPoints[j];
            if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                inside = !inside;
        }
        return inside;
    }

    public static bool IsInPolygon(Vector3[] vertices, Vector3 p)
    {
        if (vertices.Length < 3) return false;
        bool isInPolygon = false;
        var lastVertex = vertices[vertices.Length - 1];
        foreach (var vertex in vertices)
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