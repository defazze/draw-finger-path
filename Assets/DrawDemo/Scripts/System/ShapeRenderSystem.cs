
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ShapeRenderSystem : ComponentSystem
{
    private Material _shapeMaterial;
    protected override void OnCreate()
    {
        _shapeMaterial = GameManager.Instanse.shapeMaterial;
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<RenderMesh>().ForEach((Entity e, ref ShapeDetected shape) =>
        {
            Mesh mesh = null;
            var bounds = shape.bounds;
            switch (shape.type)
            {
                case ShapeType.Circle:
                    var radius = (bounds.size.x + bounds.size.y) / 4f;
                    mesh = MeshHelper.CreateCircle(radius);
                    break;
                case ShapeType.Square:
                    mesh = MeshHelper.CreateQuad(bounds.size);
                    break;
            }


            if (mesh != null)
            {
                PostUpdateCommands.AddComponent(e, new Translation { Value = bounds.center });
                PostUpdateCommands.AddComponent(e, new Rotation { Value = Quaternion.identity });
                PostUpdateCommands.AddComponent(e, new LocalToWorld());
                PostUpdateCommands.AddSharedComponent(e, new RenderMesh { mesh = mesh, material = _shapeMaterial });
            }
        });

        Entities.WithAll<InCollisionTag>().WithNone<InCollisionCompleteTag>().ForEach((Entity e, RenderMesh render) =>
        {
            var material = new Material(_shapeMaterial);
            material.color = Color.green;

            PostUpdateCommands.SetSharedComponent(e, new RenderMesh { mesh = render.mesh, material = material });
            PostUpdateCommands.AddComponent<InCollisionCompleteTag>(e);
        });
    }
}
