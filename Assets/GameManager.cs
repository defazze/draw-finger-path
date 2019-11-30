using System.Collections;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float _minDistance = .2f;
    public float trackWidth = .2f;
    private List<List<Vector3>> _tracks;
    private List<Vector3> _currentTrack;
    private EntityManager _em;
    private EntityArchetype _archetype;

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
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {


    }
}
