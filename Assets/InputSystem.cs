using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : ComponentSystem
{
    private float _minDistance;
    private float trackWidth;
    private List<Vector3> _currentTrack;
    private EntityManager _em;
    private EntityArchetype _archetype;

    protected override void OnCreate()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _archetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(TrackPoint));

        _minDistance = GameManager.Instanse._minDistance;
        trackWidth = GameManager.Instanse.trackWidth;
    }
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentTrack = new List<Vector3>();
        }

        if (Input.GetMouseButton(0))
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

            if (_currentTrack.Count > 0)
            {
                var lastPoint = _currentTrack[_currentTrack.Count - 1];
                var currentPoint = lastPoint;
                var distance = Vector3.Distance(currentPoint, point);

                while (distance > _minDistance)
                {
                    var percent = _minDistance / distance;
                    currentPoint = Vector3.Lerp(currentPoint, point, percent);
                    distance = Vector3.Distance(currentPoint, point);
                    AddPoint(currentPoint);
                }
                if (Vector3.Distance(lastPoint, point) >= _minDistance)
                {
                    AddPoint(point);
                }
            }
            else
            {
                AddPoint(point);
            }

        }
    }

    private void AddPoint(Vector3 point)
    {
        var e = _em.CreateEntity(_archetype);
        _em.SetComponentData(e, new Translation { Value = point });
        _em.SetComponentData(e, new Rotation { Value = Quaternion.identity });

        if (HasSingleton<LastPoint>())
        {
            Entities.WithAll<LastPoint>().ForEach((Entity last, ref TrackPoint lastPointQuad, ref Translation lastPointLocation) =>
            {

                float3[] vertices = new[] {
                lastPointQuad.vertice0,
                lastPointQuad.vertice1,
                lastPointQuad.vertice2,
                lastPointQuad.vertice3   };

                var ordered = vertices
                .Select(v => new { vertice = v, distance = Vector3.Distance(point, v) })
                .OrderBy(v => v.distance).ToArray();

                var edgePoint = Vector3.MoveTowards(lastPointLocation.Value, point, Vector3.Distance(lastPointLocation.Value, point) + _minDistance / 2);
                //Vector2 p=
                
                _em.RemoveComponent<LastPoint>(last);
            });

        }
        else
        {
            _em.SetComponentData(e, new TrackPoint
            {
                vertice0 = (float3)point + new float3(-_minDistance / 2, -trackWidth / 2, 0),
                vertice1 = (float3)point + new float3(_minDistance / 2, -trackWidth / 2, 0),
                vertice2 = (float3)point + new float3(-_minDistance / 2, trackWidth / 2, 0),
                vertice3 = (float3)point + new float3(_minDistance / 2, trackWidth / 2, 0)
            });
        }


        _currentTrack.Add(point);
    }
}