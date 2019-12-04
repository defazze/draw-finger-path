using Unity.Entities;
using Unity.Mathematics;

public struct Track : IComponentData
{
    public bool contrclockwise;
    public float3 connectUp;
    public float3 connectDown;
}
