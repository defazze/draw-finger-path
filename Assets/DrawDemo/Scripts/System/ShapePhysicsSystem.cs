
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Collider = Unity.Physics.Collider;

public class ShapePhysicsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<PhysicsCollider>().ForEach((Entity e, ref ShapeDetected shape) =>
        {
            var collider = GetCollider(shape.type, shape.bounds);

            var physicsMass = PhysicsMass.CreateDynamic(collider.Value.MassProperties, .2f);
            physicsMass.InverseInertia.x = 0;
            physicsMass.InverseInertia.y = 0;

            EntityManager.AddComponentData(e, new PhysicsCollider { Value = collider });
            EntityManager.AddComponentData(e, physicsMass);
            EntityManager.AddComponent<PhysicsVelocity>(e);

        });
    }

    private BlobAssetReference<Collider> GetCollider(ShapeType shapeType, ShapeBounds bounds)
    {
        if (shapeType == ShapeType.Square)
        {
            BoxGeometry boxGeometry = new BoxGeometry();
            boxGeometry.Center = Vector3.zero;
            boxGeometry.Orientation = Quaternion.identity;
            boxGeometry.Size = bounds.size + new float3(0, 0, 0.1f);
            boxGeometry.BevelRadius = .05f;

            var collider = Unity.Physics.BoxCollider.Create(boxGeometry);

            return collider;
        }

        if (shapeType == ShapeType.Circle)
        {
            var radius = (bounds.size.x + bounds.size.y) / 4f;

            CylinderGeometry geometry = new CylinderGeometry();
            geometry.Center = Vector3.zero;
            geometry.Radius = radius;
            geometry.Height = 0.1f;
            geometry.Orientation = Quaternion.identity;
            geometry.BevelRadius = .05f;
            geometry.SideCount = 20;

            var collider = Unity.Physics.CylinderCollider.Create(geometry);
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.GroupIndex = 2;
            collider.Value.Filter = collisionFilter;
            return collider;
        }

        throw new NotImplementedException($"Для фигуры {shapeType} не реализован коллайдер.");
    }
}
