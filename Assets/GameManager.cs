using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

    public Material material;
    public Material leftEdgeMaterial;
    public Material rightEdgeMaterial;
    public Material shapeMaterial;

    public static GameManager Instanse { get; private set; }

    private Mesh _correctMesh;
    private Mesh _incorrectMesh;

    private Entity _correctE;
    private Entity _incorrectE;
    private FingersImageGestureHelperComponentScript _gestureHelper;
    private EntityManager _em;
    private EntityArchetype _archetype;

    private Hash128 _id;
    private Vector3 centerShape;
    private Vector3 edgeShape;
    private List<Vector3> _points = new List<Vector3>();
    private Bounds _bounds;

    public GameManager()
    {
        Instanse = this;
        _id = new Hash128(new uint4(1, 0, 0, 0));
    }

    void Start()
    {
        //FingersScript.Instance.
        _gestureHelper = GetComponent<FingersImageGestureHelperComponentScript>();

        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        /*
        _archetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh));



        //correct mesh
        _correctE = _em.CreateEntity(_archetype);
        var mesh = QuadMesh.Create();
        var combines = new CombineInstance[3];

        combines[0].mesh = mesh;
        combines[1].mesh = mesh;
        combines[2].mesh = mesh;

        combines[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        combines[1].transform = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, Vector3.one);
        combines[2].transform = Matrix4x4.TRS(new Vector3(2, 0, 0), Quaternion.identity, Vector3.one);

        _correctMesh = new Mesh();
        _correctMesh.CombineMeshes(combines, true, true);

        _em.SetSharedComponentData(_correctE, new RenderMesh { mesh = _correctMesh, material = material });

        //incorrect mesh
        _incorrectE = _em.CreateEntity(_archetype);
        mesh = QuadMesh.Create();
        combines = new CombineInstance[3];

        combines[0].mesh = mesh;
        combines[1].mesh = mesh;
        combines[2].mesh = mesh;

        combines[0].transform = Matrix4x4.TRS(new Vector3(2, 1, 0), Quaternion.identity, Vector3.one);
        combines[1].transform = Matrix4x4.TRS(new Vector3(1, 1, 0), Quaternion.identity, Vector3.one);
        combines[2].transform = Matrix4x4.TRS(new Vector3(0, 1, 0), Quaternion.identity, Vector3.one);

        _incorrectMesh = new Mesh();
        _incorrectMesh.CombineMeshes(combines, true, true);

        _em.SetSharedComponentData(_incorrectE, new RenderMesh { mesh = _incorrectMesh, material = material });
*/
    }

    public void OnEraseToggled(bool val)
    {
        eraseMode = val;
    }

    public void OnShapeToggled(bool val)
    {
        shapeMode = val;
    }

    public void OnClick()
    {

        var entities = _em.GetAllEntities(Allocator.Temp);

        foreach (var entity in entities)
        {
            if (_em.HasComponent<ParentTrack>(entity))
            {
                _em.AddSharedComponentData(entity, new FrozenRenderSceneTag { SceneGUID = _id });
            }
        }

    }


    public void GestureCallback(GestureRecognizer gesture)
    {
        var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        if (gesture.State == GestureRecognizerState.Executing)
        {
            var point = Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, 0f));
            point.z = 0f;

            if (!_points.Contains(point))
                _points.Add(point);


            _bounds = GeometryUtility.CalculateBounds(_points.ToArray(), matrix);
        }

        if (gesture.State == GestureRecognizerState.Began)
        {
            _points.Clear();
        }
        if (gesture.State == GestureRecognizerState.Ended)
        {

        }
        else if (gesture.State != GestureRecognizerState.Began && gesture.State != GestureRecognizerState.Executing)
        {

            return;
        }
        _gestureHelper.UpdateLines();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(_bounds.min, _bounds.min + new Vector3(_bounds.size.x, 0, 0));
        Gizmos.DrawLine(_bounds.min + new Vector3(_bounds.size.x, 0, 0), _bounds.max);
        Gizmos.DrawLine(_bounds.max, _bounds.min + new Vector3(0, _bounds.size.y, 0));
        Gizmos.DrawLine(_bounds.min + new Vector3(0, _bounds.size.y, 0), _bounds.min);
    }
}