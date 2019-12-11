using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public struct PointSet : ISharedComponentData, IEquatable<PointSet>
{
    public List<float3> points;

    public bool Equals(PointSet other)
    {
        if (points == null)
        {
            return other.points == null;
        }

        return points.GetHashCode() == other.points.GetHashCode();
    }

    public override int GetHashCode()
    {
        return points.GetHashCode();

    }
}
