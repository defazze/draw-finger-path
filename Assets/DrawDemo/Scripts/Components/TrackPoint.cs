using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;

public struct TrackPoint : IComponentData
{
    public float3 normal;
    public float3 moveVector;
    public Entity track;
}
