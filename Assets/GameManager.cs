using System.Collections;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public bool eraseMode;

    public float step = .2f;
    public float trackWidth = .2f;

    public Material material;

    public static GameManager Instanse { get; private set; }

    private Mesh _newMesh;
    private Entity _e;
    private EntityManager _em;
    private EntityArchetype _archetype;
    public GameManager()
    {
        Instanse = this;
    }

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _archetype = _em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh));

        _e = _em.CreateEntity(_archetype);

        var mesh = QuadMesh.Create();
        var combines = new CombineInstance[3];

        combines[0].mesh = mesh;
        combines[1].mesh = mesh;
        combines[2].mesh = mesh;

        combines[0].transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        combines[1].transform = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, Vector3.one);
        combines[2].transform = Matrix4x4.TRS(new Vector3(2, 0, 0), Quaternion.identity, Vector3.one);

        _newMesh = new Mesh();
        _newMesh.CombineMeshes(combines, true, true);
        _newMesh.Optimize();

        _em.SetSharedComponentData(_e, new RenderMesh { mesh = _newMesh, material = material });
    }

    public void OnEraseToggled(bool val)
    {
        eraseMode = val;
    }
    
    public void OnClick()
    {
        var minTriangle = 2;
        var maxTriangle = minTriangle + 1;
        var minTriangleIndex = minTriangle * 3;
        var maxTriangleIndex = (maxTriangle + 1) * 3 - 2;

        var minVerticeIndex = _newMesh.triangles[minTriangleIndex];
        var maxVerticeIndex = _newMesh.triangles[maxTriangleIndex];

        var leftMesh = new Mesh();
        leftMesh.vertices = _newMesh.vertices.Take(minVerticeIndex).ToArray();
        leftMesh.triangles = _newMesh.triangles.Take(minTriangleIndex).ToArray();
        leftMesh.normals = _newMesh.normals.Take(minVerticeIndex).ToArray();
        leftMesh.uv = _newMesh.uv.Take(minVerticeIndex).ToArray();

        var rightMesh = new Mesh();
        rightMesh.vertices = _newMesh.vertices.Skip(maxVerticeIndex + 1).ToArray();
        rightMesh.triangles = _newMesh.triangles.Skip(maxTriangleIndex + 2).Select(t => t - (maxVerticeIndex + 1)).ToArray();
        rightMesh.normals = _newMesh.normals.Skip(maxVerticeIndex + 1).ToArray();
        rightMesh.uv = _newMesh.uv.Skip(maxVerticeIndex + 1).ToArray();

        _em.DestroyEntity(_e);

        var leftE = _em.CreateEntity(_archetype);
        var rightE = _em.CreateEntity(_archetype);

        _em.SetSharedComponentData(leftE, new RenderMesh { mesh = leftMesh, material = material });
        _em.SetSharedComponentData(rightE, new RenderMesh { mesh = rightMesh, material = material });
    }
}
