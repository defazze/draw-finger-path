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

    }
}
