using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

//[UpdateAfter(typeof(StepPhysicsWorld))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateBefore(typeof(ShapePhysicsSystem))]
unsafe public class JobCollisionSystem : JobComponentSystem
{

    [BurstCompile]
    struct MyCollisionJob : ICollisionEventsJob
    {
        //[ReadOnly] public PhysicsWorld physicsWorld;

        public void Execute(CollisionEvent ev)
        {
            Entity entityA = ev.Entities.EntityA;
            Entity entityB = ev.Entities.EntityB;
            //Entity a = physicsWorld.Bodies[ev.BodyIndices.BodyAIndex].Entity;
            //Entity b = physicsWorld.Bodies[ev.BodyIndices.BodyBIndex].Entity;
            //Debug.Log($"collision event: {ev}. Entities: {entityA}, {entityB}");
        }
    }


    BuildPhysicsWorld _buildPhysicsWorldSystem;
    StepPhysicsWorld _stepPhysicsWorld;

    protected override void OnCreate()
    {
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Debug.Log($"collision system running: {Time.ElapsedTime}"); // this runs correctly

        inputDeps = JobHandle.CombineDependencies(inputDeps, _buildPhysicsWorldSystem.FinalJobHandle);
        inputDeps = JobHandle.CombineDependencies(inputDeps, _stepPhysicsWorld.FinalJobHandle);

        var physicsWorld = _buildPhysicsWorldSystem.PhysicsWorld;

        var collisionJob = new MyCollisionJob
        {
            //physicsWorld = physicsWorld
        };
        JobHandle collisionHandle = collisionJob.Schedule(_stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);

        return collisionHandle;

    }

}

