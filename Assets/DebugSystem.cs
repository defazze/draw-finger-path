using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class DebugSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        Entities.ForEach((ref TrackMesh point, ref Translation translation) =>
        {
            var local = translation.Value;
            Debug.DrawLine(point[0] + local, point[1] + local, Color.yellow);
            Debug.DrawLine(point[1] + local, point[3] + local, Color.yellow);
            Debug.DrawLine(point[3] + local, point[2] + local, Color.yellow);
            Debug.DrawLine(point[2] + local, point[0] + local, Color.yellow);
            Debug.DrawLine(point[1] + local, point[2] + local, Color.yellow);
        });
    }
}