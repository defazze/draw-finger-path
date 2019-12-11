using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TrackEraseSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        Entities.WithAll<ErasePoint>().ForEach((Entity e, ref Translation translation) =>
        {
            var position = translation.Value;
            Entities.ForEach((Entity entityTrack, Track track) =>
            {
                var mesh = track.mesh;

                if (mesh != null)
                {
                    var inside = Poly.ContainsPoint(mesh.vertices, position, out var quadIndexes);

                    if (inside)
                    {
                        Cut(entityTrack, quadIndexes);
                    }
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
        var track = em.GetSharedComponentData<Track>(entityTrack);

        var minIndex = quadIndexes.Min();
        var maxIndex = quadIndexes.Max() + 1;

        var mesh = track.mesh;

        if (minIndex > 0)
        {
            var leftMesh = new Mesh();
            leftMesh.vertices = mesh.vertices.Take(minIndex).ToArray();
            leftMesh.triangles = mesh.triangles.Take(minIndex / 2 * 3).ToArray();
            leftMesh.normals = mesh.normals.Take(minIndex).ToArray();
            leftMesh.uv = mesh.uv.Take(minIndex).ToArray();

            var leftE = em.CreateEntity(typeof(TrackModified));
            em.AddSharedComponentData(leftE, new Track { mesh = leftMesh, contrclockwise = track.contrclockwise });
        }

        if (maxIndex < mesh.vertices.Length)
        {
            var rightMesh = new Mesh();
            rightMesh.vertices = mesh.vertices.Skip(maxIndex).ToArray();
            rightMesh.triangles = mesh.triangles.Skip(maxIndex / 2 * 3).Select(t => t - maxIndex).ToArray();
            rightMesh.normals = mesh.normals.Skip(maxIndex).ToArray();
            rightMesh.uv = mesh.uv.Skip(maxIndex).ToArray();

            var rightE = em.CreateEntity(typeof(TrackModified));
            em.AddSharedComponentData(rightE, new Track { mesh = rightMesh, contrclockwise = track.contrclockwise });
        }

        var query = GetEntityQuery(typeof(ParentTrack));
        query.SetSharedComponentFilter<ParentTrack>(new ParentTrack { track = entityTrack });
        em.DestroyEntity(query);

        em.DestroyEntity(entityTrack);
    }
}