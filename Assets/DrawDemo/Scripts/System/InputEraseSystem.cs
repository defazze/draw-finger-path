using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class InputEraseSystem : ComponentSystem
{
    private float _step;
    private List<Vector3> _currentTrack;
    private EntityManager _em;
    private EntityArchetype _pointArchetype;
    private bool _eraseMode;

    protected override void OnCreate()
    {
        _step = GameManager.Instanse.eraseStep;

        _em = EntityManager;
        _pointArchetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(ErasePoint));


    }

    protected override void OnUpdate()
    {
        _eraseMode = GameManager.Instanse.eraseMode;
        if (_eraseMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                point = new Vector3 { x = point.x, y = point.y, z = 0 };

                _currentTrack = new List<Vector3>();
            }

            if (Input.GetMouseButton(0))
            {
                var point = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

                _currentTrack.Interpolate(point, _step, AddPoint);
            }
        }
    }

    private void AddPoint(Vector3 point)
    {
        _currentTrack.Add(point);

        var e = _em.CreateEntity(_pointArchetype);
        _em.SetComponentData(e, new Translation { Value = point });
    }
}