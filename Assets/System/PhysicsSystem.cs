
using Unity.Entities;
using Unity.Physics;

public class PhysicsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<PhysicsCollider>().ForEach((Entity e, Track track) =>
        {
            PostUpdateCommands.AddComponent(e, new PhysicsCollider { Value = track.mesh.CreateCollider() });

        });
    }
}
