
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
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
        Entities.WithAll<ShapeDetected>().ForEach((Entity e, ref PhysicsCollider pCollider, ref Translation translation, ref Rotation rotation) =>
        {

            GetCollisions(e, ref pCollider, translation, rotation);

        });
    }

    private unsafe void GetCollisions(Entity e, ref PhysicsCollider pCollider, Translation translation, Rotation rotation)
    {
        _physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();

        ColliderDistanceInput distanceInput = new ColliderDistanceInput
        {
            Collider = pCollider.ColliderPtr,
            MaxDistance = .1f,
            Transform = new RigidTransform(rotation.Value, translation.Value),
        };


        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
        var collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
        if (collisionWorld.CalculateDistance(distanceInput, ref hits))
        {
            foreach (var hit in hits)
            {
                var entity = collisionWorld.Bodies[hit.RigidBodyIndex].Entity;
                if (entity != e)
                {
                    if (EntityManager.HasComponent<ShapeDetected>(entity))
                    {
                        PostUpdateCommands.AddComponent<InCollisionTag>(entity);
                    }
                }
            }
        }

        hits.Dispose();
    }
}
