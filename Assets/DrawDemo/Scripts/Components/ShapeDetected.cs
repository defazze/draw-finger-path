
using Unity.Entities;

public struct ShapeDetected : IComponentData
{
    public ShapeType type;
    public ShapeBounds bounds;
}
