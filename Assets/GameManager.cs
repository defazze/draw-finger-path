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
            typeof(RenderMesh)
        );

        var mesh1 = QuadMesh.Create();
        var mesh2 = QuadMesh.Create();
        var combines = new CombineInstance[2];

        combines[0].mesh = mesh1;
        combines[0].transform = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
        combines[1].mesh = mesh2;
        combines[1].transform = Matrix4x4.TRS(new Vector3(1, 0, 0), Quaternion.identity, new Vector3(1, 1, 1));
        var newMesh = new Mesh();
        newMesh.CombineMeshes(combines, true, true);

        var e = em.CreateEntity(archetype);
        em.SetSharedComponentData(e, new RenderMesh { mesh = newMesh, material = material });
    }
}
