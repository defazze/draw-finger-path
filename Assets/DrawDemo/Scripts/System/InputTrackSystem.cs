using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputTrackSystem : ComponentSystem
{
    private float _step;
    private float _trackWidth;
    private List<Vector3> _currentTrack;
    private bool _contrclockwise = true;
    private EntityManager _em;
    private EntityArchetype _pointArchetype;
    private Entity _currentTrackEntity;

    private bool _eraseMode;
    private bool _shapeMode;

    protected override void OnCreate()
    {
        _step = GameManager.Instanse.step;
        _trackWidth = GameManager.Instanse.trackWidth;

        _em = EntityManager;
        _pointArchetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(TrackPoint));
    }

    protected override void OnUpdate()
    {
        _eraseMode = GameManager.Instanse.eraseMode;
        _shapeMode = GameManager.Instanse.shapeMode;

        if (_shapeMode)
        {
            return;
        }

        if (!_eraseMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                point = new Vector3 { x = point.x, y = point.y, z = 0 };

                _currentTrack = new List<Vector3>();
                _currentTrackEntity = _em.CreateEntity(typeof(Track));
            }

            if (Input.GetMouseButton(0))
            {
                var point = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                _currentTrack.Interpolate(point, _step, AddPoint);

            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_currentTrack.Count == 1)
            {
                _em.DestroyEntity(_currentTrackEntity);
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
                _em.SetSharedComponentData(_currentTrackEntity, new Track { contrclockwise = _contrclockwise });
            }

            var moveVector = point - previous;
            var normal = _contrclockwise ? new Vector3(-1 * moveVector.y, moveVector.x, 0) : new Vector3(moveVector.y, -1 * moveVector.x, 0);
            normal.Normalize();
            moveVector.Normalize();

            if (_currentTrack.Count == 2)
            {
                CreatePointEntity(previous, normal, moveVector);
            }

            CreatePointEntity(point, normal, moveVector);
        }
    }

    private void CreatePointEntity(float3 point, float3 normal, float3 moveVector)
    {
        var e = _em.CreateEntity(_pointArchetype);
        _em.SetComponentData(e, new Translation { Value = point });
        _em.SetComponentData(e, new Rotation { Value = Quaternion.identity });
        _em.SetComponentData(e, new TrackPoint
        {
            normal = normal,
            moveVector = moveVector,
            track = _currentTrackEntity
        });
    }
}