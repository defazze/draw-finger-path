using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class DebugSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
/*
        Entities.ForEach((RenderMesh render, ref Translation translation) =>
        {
            var local = (Vector3)translation.Value;
            var mesh = render.mesh;


            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                var point1 = mesh.vertices[mesh.triangles[i]] + local;
                var point2 = mesh.vertices[mesh.triangles[i + 1]] + local;
                var point3 = mesh.vertices[mesh.triangles[i + 2]] + local;

                Debug.DrawLine(point1, point2, Color.yellow);
                Debug.DrawLine(point2, point3, Color.yellow);
                Debug.DrawLine(point3, point1, Color.yellow);
            }
        });*/
    }
}