using SampleGame.Spawn;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame.FloatTowards {
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct FloatingTowardsSystem : ISystem {
        [BurstCompile] public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<FloatTargetAreaTag>();
        }

        [BurstCompile] public void OnUpdate(ref SystemState state) {
            var tagEntity = SystemAPI.GetSingletonEntity<FloatTargetAreaTag>();
            var targetArea = SystemAPI.GetComponent<AreaComponentData>( tagEntity );
            var targetAreaTransform = SystemAPI.GetComponent<LocalTransform>( tagEntity );

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer( state.WorldUnmanaged );
            
            // scheduling the job to execute in parallel (multiple threads)
            new FloatingTowardsSystemJob {
                elapsedTime = SystemAPI.Time.ElapsedTime,
                targetArea = targetArea,
                targetAreaTransform = targetAreaTransform,
                physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>(),
                ecb = ecb.AsParallelWriter()
            }.ScheduleParallel();
        }


        [BurstCompile] public partial struct FloatingTowardsSystemJob : IJobEntity {
            
            [ReadOnly] public AreaComponentData targetArea;
            [ReadOnly] public double elapsedTime;
            [ReadOnly] public LocalTransform targetAreaTransform;
            [ReadOnly] public PhysicsWorldSingleton physicsWorldSingleton;
            [ReadOnly] public EntityCommandBuffer.ParallelWriter ecb;
            
            // defining our query parameters here as 'ref' or 'in'
            void Execute([ChunkIndexInQuery] int chunkIndex, [EntityIndexInChunk] int entityIndex, ref FloatTowardsComponentData floatTowards, ref PhysicsVelocity physicsVelocity,
                in PhysicsMass physicsMass, ref LocalTransform transform) 
            {
                // initialize random
                if (floatTowards.random.state == 0) {
                    floatTowards.random = new ( (uint)(elapsedTime * 100 + chunkIndex + entityIndex) );
                }

                var direction = math.normalize( floatTowards.targetPoint - transform.Position );
                
                // new random point
                if (elapsedTime > floatTowards.nextReTargetTime) {
                    floatTowards.nextReTargetTime = (float)(elapsedTime + floatTowards.reTargetRate);
                    var point = floatTowards.random.NextFloat3( -targetArea.area / 2f, targetArea.area / 2f );
                    floatTowards.targetPoint = targetAreaTransform.TransformPoint( point );
                }

                // check forward
                var input = new RaycastInput {
                    Start = transform.Position,
                    End = transform.Position + direction * 2,
                    Filter = CollisionFilter.Default
                };
                if (physicsWorldSingleton.CastRay( input )) {
                    // move backwards
                    direction = -direction;
                }
                
                // move towards point
                var moveImpulse = direction * floatTowards.speed;
                physicsVelocity.ApplyLinearImpulse( physicsMass, transform.Scale, moveImpulse );
            }
        }
    }
}