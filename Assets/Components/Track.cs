using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct Track : ISharedComponentData, IEquatable<Track>
{
    public Mesh mesh;

    public bool contrclockwise;
    public float3 connectUp;
    public float3 connectDown;

    public bool Equals(Track other)
    {
        if (mesh == null || other.mesh == null)
        {
            return false;
        }

        return object.ReferenceEquals(mesh, other.mesh);
    }

    public override int GetHashCode()
    {
        if (mesh == null)
        {
            return 0;
        }

        return mesh.GetHashCode();
    }
}
