using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

public class GameManager : MonoBehaviour
{

    public bool eraseMode;
    public bool shapeMode;
    public float step = .2f;
    public float eraseStep = .1f;
    public float trackWidth = .2f;

    public UnityEngine.Material material;
    public UnityEngine.Material leftEdgeMaterial;
    public UnityEngine.Material rightEdgeMaterial;
    public UnityEngine.Material shapeMaterial;

    public static GameManager Instanse { get; private set; }

    private Mesh _correctMesh;
    private Mesh _incorrectMesh;

    private Entity _correctE;
    private Entity _incorrectE;
    private FingersImageGestureHelperComponentScript _gestureHelper;
    private EntityManager _em;
    private EntityArchetype _archetype;

    private Hash128 _id;
    //private List<Vector3> _points = new List<Vector3>();
    private List<float3> _fPoints;
    private Bounds _bounds;
    private Entity _pointsEntity;

    public GameManager()
    {
        Instanse = this;
        _id = new Hash128(new uint4(1, 0, 0, 0));
    }

    void Start()
    {
        _gestureHelper = GetComponent<FingersImageGestureHelperComponentScript>();

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        _archetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation));
    }

    public void OnEraseToggled(bool val)
    {
        eraseMode = val;
    }

    public void OnShapeToggled(bool val)
    {
        FingersScript.Instance.ProcessUnityTouches = val;
        shapeMode = val;
    }

    public void OnClick()
    {



    }


    public void GestureCallback(GestureRecognizer gesture)
    {
        var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

        if (gesture.State == GestureRecognizerState.Began)
        {
            _pointsEntity = _em.CreateEntity();
            _fPoints = new List<float3>();
            _em.AddSharedComponentData(_pointsEntity, new PointSet { points = _fPoints });
        }

        if (gesture.State == GestureRecognizerState.Executing)
        {
            var point = (float3)Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, 0f));
            point.z = 0f;

            if (!_fPoints.Contains(point))
                _fPoints.Add(point);
        }


        if (gesture.State == GestureRecognizerState.Ended)
        {
            var imageGesture = (ImageGestureRecognizer)gesture;

            var shapeName = imageGesture.MatchedGestureImage?.Name;
            ShapeType shapeType = ShapeType.Unknown;

            switch (shapeName)
            {
                case "Circle":
                    shapeType = ShapeType.Circle;
                    break;
                case "Square":
                    shapeType = ShapeType.Square;
                    break;
            }


            if (imageGesture.MatchedGestureImage != null)
            {
                _bounds = GeometryUtility.CalculateBounds(_fPoints.Select(p => (Vector3)p).ToArray(), matrix);

                var shapeBounds = new ShapeBounds { center = _bounds.center, size = _bounds.size };
                _em.AddComponentData(_pointsEntity, new ShapeDetected { type = shapeType, bounds = shapeBounds });


            }
            _gestureHelper.Reset();
        }

        else if (gesture.State != GestureRecognizerState.Began && gesture.State != GestureRecognizerState.Executing)
        {

            return;
        }
        _gestureHelper.UpdateLines();
    }

    void OnDrawGizmos()
    {

    }
}