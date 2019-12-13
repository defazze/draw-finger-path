
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

            PostUpdateCommands.AddComponent(e, new PhysicsCollider { Value = collider });
            PostUpdateCommands.AddComponent(e, physicsMass);
            PostUpdateCommands.AddComponent<PhysicsVelocity>(e);

        });
    }

    private BlobAssetReference<Collider> GetCollider(ShapeType shapeType, ShapeBounds bounds)
    {
        var material = new Unity.Physics.Material
        {
            CustomTags = Unity.Physics.Material.Default.CustomTags,
            Flags = Unity.Physics.Material.MaterialFlags.EnableCollisionEvents |
                    Unity.Physics.Material.MaterialFlags.EnableMassFactors |
                    Unity.Physics.Material.MaterialFlags.EnableSurfaceVelocity,
            Friction = Unity.Physics.Material.Default.Friction,
            FrictionCombinePolicy = Unity.Physics.Material.Default.FrictionCombinePolicy,
            Restitution = Unity.Physics.Material.Default.Restitution,
            RestitutionCombinePolicy = Unity.Physics.Material.Default.RestitutionCombinePolicy,
        };

        var collider = BlobAssetReference<Collider>.Null;
        if (shapeType == ShapeType.Square)
        {
            BoxGeometry boxGeometry = new BoxGeometry();
            boxGeometry.Center = Vector3.zero;
            boxGeometry.Orientation = Quaternion.identity;
            boxGeometry.Size = bounds.size + new float3(0, 0, 0.1f);
            boxGeometry.BevelRadius = .05f;

            collider = Unity.Physics.BoxCollider.Create(boxGeometry, CollisionFilter.Default, material);
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

            collider = Unity.Physics.CylinderCollider.Create(geometry, CollisionFilter.Default, material);
        }

        if (collider == BlobAssetReference<Collider>.Null)
        {
            throw new NotImplementedException($"Для фигуры {shapeType} не реализован коллайдер.");
        }


        return collider;
    }
}
