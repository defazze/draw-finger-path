using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : ComponentSystem
{
    private float _step;
    private float _trackWidth;
    private List<Vector3> _currentTrack;
    private bool _contrclockwise = true;
    private EntityManager _em;
    private EntityArchetype _archetype;
    private Entity _currentEntity;

    protected override void OnCreate()
    {
        _step = GameManager.Instanse.step;
        _trackWidth = GameManager.Instanse.trackWidth;

        _em = EntityManager;
        _archetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(TrackPoint));
    }

    protected override void OnUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            _currentTrack = new List<Vector3>();
            _currentEntity = Entity.Null;
        }

        if (Input.GetMouseButton(0))
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

            if (_currentTrack.Count > 0)
            {
                var lastPoint = _currentTrack[_currentTrack.Count - 1];
                var currentPoint = lastPoint;
                var distance = Vector3.Distance(currentPoint, point);

                while (distance > _step)
                {
                    var percent = _step / distance;
                    currentPoint = Vector3.Lerp(currentPoint, point, percent);
                    distance = Vector3.Distance(currentPoint, point);
                    AddPoint(currentPoint);
                }
                if (Vector3.Distance(lastPoint, point) >= _step)
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
        _currentTrack.Add(point);

        if (_currentTrack.Count > 1)
        {
            var previous = _currentTrack[_currentTrack.Count - 2];

            if (_currentTrack.Count == 2)
            {
                _contrclockwise = _currentTrack[0].x <= _currentTrack[1].x;
            }

            var moveVector = point - previous;
            var normal = _contrclockwise ? new Vector3(-1 * moveVector.y, moveVector.x, 0) : new Vector3(moveVector.y, -1 * moveVector.x, 0);
            normal.Normalize();
            moveVector.Normalize();

            if (_currentTrack.Count == 2)
            {
                CreateEntity(previous, normal, moveVector);
            }

            CreateEntity(point, normal, moveVector);
        }
    }

    private void CreateEntity(float3 point, float3 normal, float3 moveVector)
    {
        var e = _em.CreateEntity(_archetype);
        _em.SetComponentData(e, new Translation { Value = point });
        _em.SetComponentData(e, new Rotation { Value = Quaternion.identity });
        _em.SetComponentData(e, new TrackPoint { normal = normal, moveVector = moveVector, previous = _currentEntity });

        if (_em.Exists(_currentEntity))
        {
            var previosPoint = _em.GetComponentData<TrackPoint>(_currentEntity);
            previosPoint.next = e;
            _em.SetComponentData(_currentEntity, previosPoint);
        }

        _currentEntity = e;
    }
}