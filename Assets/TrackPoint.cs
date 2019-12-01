using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;

public struct TrackPoint : IComponentData
{
    public float3 normal;
    public float3 moveVector;
    public Entity previous;
    public bool contrclockwise;
}

public struct TrackMesh : IComponentData
{

    public float3 vertice0;
    public float3 vertice1;
    public float3 vertice2;
    public float3 vertice3;

    public float3 this[int i]
    {
        get
        {
            switch (i)
            {
                case 0: return vertice0;
                case 1: return vertice1;
                case 2: return vertice2;
                case 3: return vertice3;
                default: throw new IndexOutOfRangeException();
            }
        }
        set
        {
            switch (i)
            {
                case 0: vertice0 = value; break;
                case 1: vertice1 = value; break;
                case 2: vertice2 = value; break;
                case 3: vertice3 = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public void AddRange(IEnumerable<float3> vertices)
    {
        var list = vertices.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            this[i] = list[i];
        }
    }

    public IEnumerable<float3> Vertices
    {
        get
        {
            yield return vertice0;
            yield return vertice1;
            yield return vertice2;
            yield return vertice3;
        }
    }

    public int IndexOf(float3 vertice)
    {
        for (int i = 0; i < 3; i++)
        {
            if (this[i].Equals(vertice))
            {
                return i;
            }
        }

        return -1;
    }
    public DynamicBuffer<float3> vertices;
}