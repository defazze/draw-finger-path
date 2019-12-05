using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class TrackRenderSystem : ComponentSystem
{
    private EntityArchetype _trackArchetype;
    private EntityArchetype _renderArchetype;
    protected override void OnCreate()
    {
        _trackArchetype = EntityManager.CreateArchetype(typeof(Track));

        _renderArchetype = EntityManager.CreateArchetype(typeof(LocalToWorld),
                                                        typeof(Translation),
                                                        typeof(Rotation),
                                                        typeof(RenderMesh),
                                                        typeof(Parent));
    }
    protected override void OnUpdate()
    {
        Entities.WithAll<TrackModified>().ForEach((Entity e, Track track) =>
        {
            var newEntity = EntityManager.CreateEntity(_trackArchetype);
            //EntityManager.AddBuffer<LinkedEntityGroup>(newEntity);
            EntityManager.SetSharedComponentData<Track>(newEntity, track);

            var renderEntity = EntityManager.CreateEntity(_renderArchetype);
            EntityManager.SetComponentData<Parent>(renderEntity, new Parent { Value = newEntity });

            //var buffer = EntityManager.GetBuffer<LinkedEntityGroup>(newEntity);
            //buffer.Add(renderEntity);

            PostUpdateCommands.DestroyEntity(e);
        });
    }
}
