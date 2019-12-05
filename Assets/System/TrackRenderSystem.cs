using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class TrackRenderSystem : ComponentSystem
{
    private EntityArchetype _trackArchetype;
    private EntityArchetype _renderArchetype;
    private Material _material;

    protected override void OnCreate()
    {
        _material = GameManager.Instanse.material;
        _trackArchetype = EntityManager.CreateArchetype(typeof(Track));

        _renderArchetype = EntityManager.CreateArchetype(typeof(LocalToWorld),
                                                        typeof(Translation),
                                                        typeof(Rotation),
                                                        typeof(RenderMesh));
    }
    protected override void OnUpdate()
    {
        Entities.WithAll<TrackModified>().ForEach((Entity e, Track track) =>
        {
            var query = GetEntityQuery(typeof(ParentTrack));
            query.SetSharedComponentFilter<ParentTrack>(new ParentTrack { track = e });
            var oldRenders = query.ToEntityArray(Allocator.TempJob);

            PostUpdateCommands.RemoveComponent<TrackModified>(e);
            for (int i = 0; i < oldRenders.Length; i++)
            {
                PostUpdateCommands.DestroyEntity(oldRenders[i]);
            }

            oldRenders.Dispose();

            var renderEntity = EntityManager.CreateEntity(_renderArchetype);
            EntityManager.AddSharedComponentData<ParentTrack>(renderEntity, new ParentTrack { track = e });
            EntityManager.SetSharedComponentData<RenderMesh>(renderEntity, new RenderMesh { mesh = track.mesh, material = _material });
        });
    }
}
