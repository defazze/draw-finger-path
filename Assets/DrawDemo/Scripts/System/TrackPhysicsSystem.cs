
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TrackRenderSystem))]
[UpdateAfter(typeof(TrackBuildSystem))]
public class TrackPhysicsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TrackModified>().ForEach((Entity e, Track track) =>
        {
            if (track.mesh != null)
            {
                PostUpdateCommands.AddComponent(e, new Translation { Value = Vector3.zero });
                PostUpdateCommands.AddComponent(e, new Rotation { Value = Quaternion.identity });
                PostUpdateCommands.AddComponent(e, new LocalToWorld());

                if (EntityManager.HasComponent<PhysicsCollider>(e))
                {
                    PostUpdateCommands.RemoveComponent<PhysicsCollider>(e);
                }

                var collisionFilter = CollisionFilter.Default;
                collisionFilter.GroupIndex = 1;

                var collider = track.mesh.CreateCollider();
                collider.Value.Filter = collisionFilter;
                var isValid = collisionFilter.IsValid;
                PostUpdateCommands.AddComponent(e, new PhysicsCollider { Value = collider });
            }
        });
    }
}
