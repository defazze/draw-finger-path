using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class TrackRenderSystem : ComponentSystem
{
    private Mesh _mesh;

    protected override void OnCreate()
    {
        var width = GameManager.Instanse.trackWidth;
        var height = GameManager.Instanse.trackWidth;
        var vertices = new Vector3[4]
{
            new Vector3(-width/2, -height/2, 0),
            new Vector3(width/2, -height/2, 0),
            new Vector3(-width/2, height/2, 0),
            new Vector3(width/2, width/2, 0)
};
        _mesh = QuadMesh.Create(vertices);
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<TrackPoint>().WithNone<Displayed>().ForEach((Entity e, ref Translation translation) =>
        {
            RenderMesh render = new RenderMesh { mesh = _mesh, material = GameManager.Instanse.material };
            PostUpdateCommands.AddSharedComponent(e, render);
            PostUpdateCommands.AddComponent(e, new Displayed());
        });
    }
}