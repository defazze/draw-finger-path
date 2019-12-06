using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

public class TrackRenderSystem : ComponentSystem
{
    private EntityArchetype _trackArchetype;
    private EntityArchetype _renderArchetype;
    private Hash128 _id;
    private Material _material;
    private Material _leftMaterial;
    private Material _rightMaterial;

    protected override void OnCreate()
    {
        _id = new Hash128(new uint4(1, 0, 0, 0));

        _material = GameManager.Instanse.material;
        _leftMaterial = GameManager.Instanse.leftEdgeMaterial;
        _rightMaterial = GameManager.Instanse.rightEdgeMaterial;

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

            EntityManager.DestroyEntity(query);

            var mainMesh = track.mesh;
            var quads = mainMesh.vertices.Length / 4;

            var combines = new CombineInstance[3];

            combines[0].mesh = mainMesh.Take();
            combines[1].mesh = mainMesh.Take(1, quads - 2);
            combines[2].mesh = mainMesh.Take(quads - 1);

            var renderMesh = new Mesh();
            renderMesh.CombineMeshes(combines, false, false);

            var leftIndex = track.contrclockwise ? 0 : 2;
            var rightIndex = track.contrclockwise ? 2 : 0;

            CreateRender(e, renderMesh, leftIndex, _leftMaterial);
            CreateRender(e, renderMesh, 1, _material);
            CreateRender(e, renderMesh, rightIndex, _rightMaterial);

            PostUpdateCommands.RemoveComponent<TrackModified>(e);
        });
    }

    private void CreateRender(Entity track, Mesh mesh, int subMesh, Material material)
    {
        var renderEntity = EntityManager.CreateEntity(_renderArchetype);
        EntityManager.AddSharedComponentData<ParentTrack>(renderEntity, new ParentTrack { track = track });
        EntityManager.SetSharedComponentData<RenderMesh>(renderEntity, new RenderMesh { mesh = mesh, subMesh = subMesh, material = material });
        EntityManager.AddSharedComponentData(renderEntity, new FrozenRenderSceneTag { SceneGUID = _id });
    }
}
