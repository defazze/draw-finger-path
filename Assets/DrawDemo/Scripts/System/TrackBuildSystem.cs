using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TrackBuildSystem : ComponentSystem
{

    private float _step;
    private float _trackWidth;

    protected override void OnCreate()
    {
        _step = GameManager.Instanse.step;
        _trackWidth = GameManager.Instanse.trackWidth;
    }

    protected override void OnUpdate()
    {

        Entities.WithNone<ErasePoint>().ForEach((Entity e, ref Translation translation, ref TrackPoint trackPoint) =>
        {
            var em = EntityManager;
            var pointLocate = translation.Value;
            var mesh = new Mesh();
            var vertices = new List<float3>(4);

            var meshVertices = new Vector3[4];
            var moveVector = Vector3.ClampMagnitude(trackPoint.moveVector, _step / 2);
            var normal = Vector3.ClampMagnitude(trackPoint.normal, _trackWidth / 2);

            int freeUpIndex = 3;
            int freeDownIndex = 1;
            int connectedUpIndex = 2;
            int connectedDownIndex = 0;

            var trackEntity = trackPoint.track;
            var track = em.GetSharedComponentData<Track>(trackEntity);
            var contrclockwise = track.contrclockwise;
            if (!contrclockwise)
            {
                freeUpIndex = 2;
                freeDownIndex = 0;
                connectedUpIndex = 3;
                connectedDownIndex = 1;
            }

            meshVertices[freeUpIndex] = moveVector + normal + (Vector3)pointLocate;
            meshVertices[freeDownIndex] = moveVector - normal + (Vector3)pointLocate;

            if (track.mesh == null)
            {
                meshVertices[connectedUpIndex] = normal - moveVector + (Vector3)pointLocate;
                meshVertices[connectedDownIndex] = -1 * normal - moveVector + (Vector3)pointLocate;

                mesh = MeshHelper.CreateQuad(meshVertices);

                track.connectUp = meshVertices[freeUpIndex];
                track.connectDown = meshVertices[freeDownIndex];
                track.mesh = mesh;

                em.SetSharedComponentData(trackEntity, track);
            }
            else
            {
                var mainMesh = track.mesh;

                meshVertices[connectedUpIndex] = track.connectUp;
                meshVertices[connectedDownIndex] = track.connectDown;

                track.connectUp = meshVertices[freeUpIndex];
                track.connectDown = meshVertices[freeDownIndex];

                mesh = MeshHelper.CreateQuad(meshVertices);

                var combines = new CombineInstance[2];
                combines[0].mesh = mainMesh;
                combines[1].mesh = mesh;

                var newMesh = new Mesh();
                newMesh.CombineMeshes(combines, true, false);
                track.mesh = newMesh;

                em.SetSharedComponentData(trackEntity, track);
                em.AddComponent<TrackModified>(trackEntity);
            }

            PostUpdateCommands.DestroyEntity(e);
        });
    }
}