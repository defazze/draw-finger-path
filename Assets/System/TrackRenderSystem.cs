using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class TrackRenderSystem : ComponentSystem
{
    private Mesh _standartMesh;
    private float _step;
    private float _trackWidth;
    private Vector3[] _standartVertices;
    private RenderMesh _standartRender;
    protected override void OnCreate()
    {
        _step = GameManager.Instanse.step;
        _trackWidth = GameManager.Instanse.trackWidth;

        _standartVertices = new Vector3[4]
       {
            new Vector3(-_trackWidth/2, -_step/2, 0),
            new Vector3(_trackWidth/2, -_step/2, 0),
            new Vector3(-_trackWidth/2, _step/2, 0),
            new Vector3(_trackWidth/2, _step/2, 0)
       };


        _standartMesh = QuadMesh.Create(_standartVertices);
        _standartRender = new RenderMesh { mesh = _standartMesh, material = GameManager.Instanse.material };
    }

    protected override void OnUpdate()
    {

        Entities.WithNone<ErasePoint>().ForEach((Entity e, ref Translation translation, ref TrackPoint trackPoint) =>
        {
            var em = EntityManager;
            var myLocate = translation.Value;
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
            var track = em.GetComponentData<Track>(trackEntity);
            var contrclockwise = track.contrclockwise;
            if (!contrclockwise)
            {

                freeUpIndex = 2;
                freeDownIndex = 0;
                connectedUpIndex = 3;
                connectedDownIndex = 1;
            }

            meshVertices[freeUpIndex] = moveVector + normal;
            meshVertices[freeDownIndex] = moveVector - normal;
            var tempConnectedUp = normal - moveVector;
            var tempConnectedDown = -1 * normal - moveVector;

            var mainLocate = em.GetComponentData<Translation>(trackEntity).Value;

            if (em.HasComponent<RenderMesh>(trackEntity))
            {
                var mainMesh = em.GetSharedComponentData<RenderMesh>(trackEntity).mesh;

                meshVertices[connectedUpIndex] = track.connectUp - myLocate;
                meshVertices[connectedDownIndex] = track.connectDown - myLocate;

                track.connectUp = (float3)meshVertices[freeUpIndex] + myLocate;
                track.connectDown = (float3)meshVertices[freeDownIndex] + myLocate;
                em.SetComponentData(trackEntity, track);

                mesh = QuadMesh.Create(meshVertices);

                var combines = new CombineInstance[2];
                combines[0].mesh = mainMesh;
                combines[0].transform = Matrix4x4.TRS(mainLocate, Quaternion.identity, Vector3.one);
                combines[1].mesh = mesh;
                combines[1].transform = Matrix4x4.TRS(myLocate, Quaternion.identity, Vector3.one);

                var newMesh = new Mesh();
                newMesh.CombineMeshes(combines, true, true);

                em.SetComponentData(trackEntity, new Translation { Value = Vector3.zero });
                em.SetSharedComponentData(trackEntity, new RenderMesh { mesh = newMesh, material = GameManager.Instanse.material });
            }
            else
            {
                meshVertices[connectedUpIndex] = tempConnectedUp;
                meshVertices[connectedDownIndex] = tempConnectedDown;

                mesh = QuadMesh.Create(meshVertices);
                em.AddSharedComponentData(trackEntity, new RenderMesh { mesh = mesh, material = GameManager.Instanse.material });

                track.connectUp = (float3)meshVertices[freeUpIndex] + myLocate;
                track.connectDown = (float3)meshVertices[freeDownIndex] + myLocate;
                em.SetComponentData(trackEntity, track);
            }

            PostUpdateCommands.DestroyEntity(e);
        });
    }
}