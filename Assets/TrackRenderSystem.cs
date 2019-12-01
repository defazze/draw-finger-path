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
    }

    protected override void OnUpdate()
    {
        var em = EntityManager;
        Entities.WithNone<TrackMesh>().ForEach((Entity e, ref Translation translation, ref TrackPoint trackPoint) =>
        {
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

            if (!trackPoint.contrclockwise)
            {

                freeUpIndex = 2;
                freeDownIndex = 0;
                connectedUpIndex = 3;
                connectedDownIndex = 1;
            }

            meshVertices[freeUpIndex] = moveVector + normal;
            meshVertices[freeDownIndex] = moveVector - normal;

            if (em.Exists(trackPoint.previous))
            {
                var previousPoint = em.GetComponentData<TrackMesh>(trackPoint.previous);
                var previousLocate = em.GetComponentData<Translation>(trackPoint.previous).Value;
                meshVertices[connectedUpIndex] = previousPoint[freeUpIndex] + previousLocate - myLocate;
                meshVertices[connectedDownIndex] = previousPoint[freeDownIndex] + previousLocate - myLocate;
            }
            else
            {
                meshVertices[connectedUpIndex] = normal - moveVector;
                meshVertices[connectedDownIndex] = -1 * normal - moveVector;
            }

            mesh = QuadMesh.Create(meshVertices);

            var trackMesh = new TrackMesh();
            trackMesh.AddRange(meshVertices.Select(v => (float3)v));

            RenderMesh render = new RenderMesh { mesh = mesh, material = GameManager.Instanse.material };
            em.AddSharedComponentData(e, render);
            em.AddComponentData(e, trackMesh);

        });
    }
}