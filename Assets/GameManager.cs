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
    public float eraseStep = .1f;
    public float trackWidth = .2f;

    public Material material;

    public static GameManager Instanse { get; private set; }

    private Mesh _correctMesh;
    private Mesh _incorrectMesh;

    private Entity _correctE;
    private Entity _incorrectE;
    private EntityManager _em;
    private EntityArchetype _archetype;
    public GameManager()
    {
        Instanse = this;
    }

    void Start()
    {
        /*
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
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

    public void OnClick()
    {
        Cut(_correctE, new[] { 4, 5, 6, 7 });
        Cut(_incorrectE, new[] { 4, 5, 6, 7 });
    }

    private void Cut(Entity e, int[] verticeIndexes)
    {
        var minVerticeIndex = verticeIndexes.Min();
        var maxVerticeIndex = verticeIndexes.Max() + 1;

        var mesh = _em.GetSharedComponentData<RenderMesh>(e).mesh;

        var leftMesh = new Mesh();
        leftMesh.vertices = mesh.vertices.Take(minVerticeIndex).ToArray();
        leftMesh.triangles = mesh.triangles.Take(minVerticeIndex / 2 * 3).ToArray();
        leftMesh.normals = mesh.normals.Take(minVerticeIndex).ToArray();
        leftMesh.uv = mesh.uv.Take(minVerticeIndex).ToArray();

        var rightMesh = new Mesh();
        rightMesh.vertices = mesh.vertices.Skip(maxVerticeIndex).ToArray();
        rightMesh.triangles = mesh.triangles.Skip(maxVerticeIndex / 2 * 3).Select(t => t - maxVerticeIndex).ToArray();
        rightMesh.normals = mesh.normals.Skip(maxVerticeIndex).ToArray();
        rightMesh.uv = mesh.uv.Skip(maxVerticeIndex).ToArray();

        _em.DestroyEntity(e);

        var leftE = _em.CreateEntity(_archetype);
        var rightE = _em.CreateEntity(_archetype);

        _em.SetSharedComponentData(leftE, new RenderMesh { mesh = leftMesh, material = material });
        _em.SetSharedComponentData(rightE, new RenderMesh { mesh = rightMesh, material = material });
    }
}
