
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[UpdateAfter(typeof(RenderMeshSystemV2))]
public class FrozenSystem : ComponentSystem
{
    private Hash128 _id = new Hash128(new uint4(1, 0, 0, 0));
    protected override void OnCreate()
    {
        _id = new Hash128(new uint4(1, 0, 0, 0));
    }
    protected override void OnUpdate()
    {
        var frozen = new FrozenRenderSceneTag { SceneGUID = _id };
        Entities.WithAll<ParentTrack>().WithNone<FrozenRenderSceneTag>().ForEach((Entity e) =>
        {
            PostUpdateCommands.AddSharedComponent(e, frozen);
        });
    }
}
