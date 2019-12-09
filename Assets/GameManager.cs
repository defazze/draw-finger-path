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
        var point = Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, 0f));
        point.z = 0f;

        if (!_points.Contains(point))
            _points.Add(point);

        var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        _bounds = GeometryUtility.CalculateBounds(_points.ToArray(), matrix);

        /*
                _bounds = new Bounds(_points[0], Vector3.zero);



                for (int i = 1; i < _points.Count; i++)
                {
                    _bounds.Encapsulate(new Bounds(_points[i], Vector3.zero));
                }
        */

        if (gesture.State == GestureRecognizerState.Ended)
        {
            _points.Clear();
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

        foreach (var point in _points)
        {
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(point, Vector3.forward, .2f);
        }
        Gizmos.DrawWireCube(_bounds.center, _bounds.extents);
    }

    private Bounds GetBounds(Vector3[] points)
    {

        Vector3 xMin = new Vector3(100, 0, 0);
        Vector3 xMax = new Vector3(-100, 0, 0);
        Vector3 yMin = new Vector3(0, 100, 0);
        Vector3 yMax = new Vector3(0, -100, 0);

        foreach (var point in points)
        {
            if (point.x < xMin.x)
            {
                xMin = point;
            }
            if (point.x > xMax.x)
            {
                xMax = point;
            }
            if (point.y < yMin.y)
            {
                yMin = point;
            }
            if (point.y > yMax.y)
            {
                yMax = point;
            }
        }
        var sizeX = xMax.x - xMin.x;
        var sizeY = yMax.y - yMin.y;

        //var center=new Vector3(xMin+sizeX/2f,yMin+sizeY/2f,0)
        var result = new Bounds();
        return result;
    }
}