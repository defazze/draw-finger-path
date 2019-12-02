using System.Collections;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;

public class GameManager : MonoBehaviour
{

    public float step = .2f;
    public float trackWidth = .2f;

    public Material material;

    public static GameManager Instanse { get; private set; }

    private Mesh _newMesh;
    public GameManager()
    {
        Instanse = this;
    }

    void Start()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var archetype = em.CreateArchetype(
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh));

        var e = em.CreateEntity(archetype);

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

        em.SetSharedComponentData(e, new RenderMesh { mesh = _newMesh, material = material });
    }

    public void OnClick()
    {
        //var minTriangleIndex = 2;

        var mesh = _newMesh;
    }
}
