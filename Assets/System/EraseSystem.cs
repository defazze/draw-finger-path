using System.Linq;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class EraseSystem : ComponentSystem
{
    private EntityArchetype _archetype;
    private Material _material;
    protected override void OnCreate()
    {
        _archetype = EntityManager.CreateArchetype(
    typeof(LocalToWorld),
    typeof(Translation),
    typeof(Rotation),
    typeof(Track),
    typeof(RenderMesh));

        _material = GameManager.Instanse.material;
    }

    protected override void OnUpdate()
    {

        Entities.WithAll<ErasePoint>().ForEach((Entity e, ref Translation translation) =>
        {
            var position = translation.Value;
            Entities.WithAll<Track>().ForEach((Entity entityTrack, RenderMesh render) =>
            {
                var mesh = render.mesh;

                var inside = Poly.ContainsPoint(mesh.vertices, position, out var quadIndexes);

                if (inside)
                {
                    Cut(entityTrack, quadIndexes);
                }

            });
        });

        Entities.WithAll<ErasePoint>().ForEach((Entity e) =>
        {
            PostUpdateCommands.DestroyEntity(e);
        });

    }

    private void Cut(Entity entityTrack, int[] quadIndexes)
    {
        var em = EntityManager;

        var minIndex = quadIndexes.Min();
        var maxIndex = quadIndexes.Max() + 1;

        var mesh = em.GetSharedComponentData<RenderMesh>(entityTrack).mesh;

        if (minIndex > 0)
        {
            var leftMesh = new Mesh();
            leftMesh.vertices = mesh.vertices.Take(minIndex).ToArray();
            leftMesh.triangles = mesh.triangles.Take(minIndex / 2 * 3).ToArray();
            leftMesh.normals = mesh.normals.Take(minIndex).ToArray();
            leftMesh.uv = mesh.uv.Take(minIndex).ToArray();

            var leftE = em.CreateEntity(_archetype);
            em.SetSharedComponentData(leftE, new RenderMesh { mesh = leftMesh, material = _material });
        }

        if (maxIndex < mesh.vertices.Length)
        {
            var rightMesh = new Mesh();
            rightMesh.vertices = mesh.vertices.Skip(maxIndex).ToArray();
            rightMesh.triangles = mesh.triangles.Skip(maxIndex / 2 * 3).Select(t => t - maxIndex).ToArray();
            rightMesh.normals = mesh.normals.Skip(maxIndex).ToArray();
            rightMesh.uv = mesh.uv.Skip(maxIndex).ToArray();

            var rightE = em.CreateEntity(_archetype);
            em.SetSharedComponentData(rightE, new RenderMesh { mesh = rightMesh, material = _material });
        }

        if (em.Exists(entityTrack))
        {
            em.DestroyEntity(entityTrack);
        }
    }
}