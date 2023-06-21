using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame {
    // runs every fixed update
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial struct MoveSystem : ISystem {

        [BurstCompile] public void OnUpdate(ref SystemState state) {
            // get inputs using old Input system
            var forward = Input.GetAxis( "Vertical" ) * SystemAPI.Time.DeltaTime;
            var rotate = Input.GetAxis( "Horizontal" ) * SystemAPI.Time.DeltaTime;

            // iterate over all entities that have MovingComponentData, LocalTransform, PhysicsVelocity and PhysicsMass
            foreach (var (moving, transform, physicsVelocity, physicsMass) in SystemAPI.Query<
                         RefRO<MovingComponentData>, RefRW<LocalTransform>,
                         RefRW<PhysicsVelocity>, RefRW<PhysicsMass>>()) 
            {
                // assign force to move object
                var moveImpulse = transform.ValueRW.Forward() * forward * moving.ValueRO.moveSpeed;
                physicsVelocity.ValueRW.ApplyLinearImpulse( physicsMass.ValueRO, moveImpulse );

                // assign force to rotate object
                var rotationImpulse = rotate * moving.ValueRO.rotateSpeed;
                physicsVelocity.ValueRW.ApplyAngularImpulse( physicsMass.ValueRO, new float3( 0, rotationImpulse, 0 ) );
            }
        }
    }
}