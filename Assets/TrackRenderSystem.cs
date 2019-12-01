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
        var height = _step;

        _standartVertices = new Vector3[4]
       {
            new Vector3(-_trackWidth/2, -height/2, 0),
            new Vector3(_trackWidth/2, -height/2, 0),
            new Vector3(-_trackWidth/2, height/2, 0),
            new Vector3(_trackWidth/2, height/2, 0)
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

            if (em.Exists(trackPoint.previous))
            {
                var previousLocate = em.GetComponentData<Translation>(trackPoint.previous).Value;
                if (em.HasComponent<TrackMesh>(trackPoint.previous))
                {
                    var previousMesh = em.GetComponentData<TrackMesh>(trackPoint.previous);
                    meshVertices[0] = previousMesh[1] + previousLocate - myLocate;
                    meshVertices[2] = previousMesh[3] + previousLocate - myLocate;
                }
                else
                {
                    var points = GetMedianNormals(myLocate, previousLocate);
                    meshVertices[0] = points[0];
                    meshVertices[2] = points[1];
                }
            }
            else
            {
                meshVertices[0] = -1 * moveVector - normal;
                meshVertices[2] = -1 * moveVector + normal;
            }

            if (em.Exists(trackPoint.next))
            {
                var nextLocate = em.GetComponentData<Translation>(trackPoint.next).Value;
                if (em.HasComponent<TrackMesh>(trackPoint.next))
                {
                    var nextMesh = em.GetComponentData<TrackMesh>(trackPoint.next);
                    meshVertices[1] = nextMesh[0] + nextLocate - myLocate;
                    meshVertices[3] = nextMesh[2] + nextLocate - myLocate;
                }
                else
                {
                    var points = GetMedianNormals(myLocate, nextLocate);

                    meshVertices[1] = points[1];
                    meshVertices[3] = points[0];
                }
            }
            else
            {
                meshVertices[1] = moveVector - normal;
                meshVertices[3] = moveVector + normal;
            }

            mesh = QuadMesh.Create(meshVertices);

            var trackMesh = new TrackMesh();
            trackMesh.AddRange(meshVertices.Select(v => (float3)v));

            RenderMesh render = new RenderMesh { mesh = mesh, material = GameManager.Instanse.material };
            em.AddSharedComponentData(e, render);
            em.AddComponentData(e, trackMesh);

        });
    }

    protected float3[] GetMedianNormals(float3 first, float3 second)
    {
        var vector = second - first;
        var normal = new float3(-vector.y, vector.x, 0);
        var normalized = Vector3.Normalize(normal);
        normalized = Vector3.ClampMagnitude(normalized, _trackWidth / 2);

        var halfVector = (float3)Vector3.Lerp(Vector3.zero, vector, .5f);

        normal = halfVector + (float3)normalized;
        var antinormal = halfVector - (float3)normalized;

        return new[] { (float3)normal, antinormal };
    }
}