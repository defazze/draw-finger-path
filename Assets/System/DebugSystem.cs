using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class DebugSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        Entities.ForEach((ref PhysicsCollider collider, ref Translation translation) =>
        {
            var temp = collider;
            var aabb = temp.Value.Value.CalculateAabb();
            var transform = translation.Value;

            Debug.DrawLine(aabb.Min + transform, new float3(aabb.Max.x, aabb.Min.y, 0) + transform, Color.yellow);
            Debug.DrawLine(new float3(aabb.Max.x, aabb.Min.y, 0) + transform, aabb.Max + transform, Color.yellow);
            Debug.DrawLine(aabb.Max + transform, new float3(aabb.Min.x, aabb.Max.y, 0) + transform, Color.yellow);
            Debug.DrawLine(new float3(aabb.Min.x, aabb.Max.y, 0) + transform, aabb.Min + transform, Color.yellow);
        });
    }
}