
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class CollisionSystem : ComponentSystem
{
    private BuildPhysicsWorld _physicsWorldSystem;
    private CollisionWorld _collisionWorld;

    protected override void OnCreate()
    {


    }
    protected override void OnUpdate()
    {
        Entities.WithAll<ShapeDetected>().ForEach((ref PhysicsCollider pCollider, ref Translation translation, ref Rotation rotation) =>
        {

            GetCollisions(ref pCollider, translation, rotation);

        });
    }

    private unsafe void GetCollisions(ref PhysicsCollider pCollider, Translation translation, Rotation rotation)
    {
        _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();

        ColliderDistanceInput distanceInput = new ColliderDistanceInput
        {
            Collider = pCollider.ColliderPtr,
            MaxDistance = 10f,
            Transform = new RigidTransform(rotation.Value, translation.Value),
        };


        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
        AllHitsCollector<DistanceHit> collector = new AllHitsCollector<DistanceHit>(10f, ref hits);
        var collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
        if (collisionWorld.CalculateDistance(distanceInput, ref collector))
        {
            foreach (var hit in collector.AllHits)
            {
                Debug.DrawLine(translation.Value, hit.Position, Color.yellow);
            }
        }

        hits.Dispose();
    }
}
