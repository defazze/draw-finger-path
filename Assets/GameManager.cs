using System.Collections;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

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
    void Awake()
    {
        Instanse = this;
    }
}
