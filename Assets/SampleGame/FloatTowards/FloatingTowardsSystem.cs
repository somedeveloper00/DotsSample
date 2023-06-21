using SampleGame.Spawn;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

namespace SampleGame.FloatTowards {
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct FloatingTowardsSystem : ISystem {
        
        [BurstCompile] public void OnCreate(ref SystemState state) => 
            state.RequireForUpdate<FloatTargetAreaTag>();

        [BurstCompile] public void OnUpdate(ref SystemState state) {
            var tagEntity = SystemAPI.GetSingletonEntity<FloatTargetAreaTag>();
            var targetArea = SystemAPI.GetComponent<AreaComponentData>( tagEntity );
            var targetAreaTransform = SystemAPI.GetComponent<LocalTransform>( tagEntity );
            
            // scheduling the job to execute in parallel (multiple threads)
            new FloatingTowardsSystemJob {
                elapsedTime = SystemAPI.Time.ElapsedTime,
                targetArea = targetArea,
                targetAreaTransform = targetAreaTransform
            }.ScheduleParallel();
        }


        [BurstCompile] public partial struct FloatingTowardsSystemJob : IJobEntity {
            
            [ReadOnly] public AreaComponentData targetArea;
            public LocalTransform targetAreaTransform;
            public double elapsedTime;
            
            // defining our query parameters here as 'ref' or 'in'
            [BurstCompile] void Execute(
                ref FloatTowardsComponentData floatTowards, ref PhysicsVelocity physicsVelocity,
                in PhysicsMass physicsMass, ref LocalTransform transform) 
            {
                // new random point
                if (elapsedTime > floatTowards.nextReTargetTime) {
                    floatTowards.nextReTargetTime = (float)(elapsedTime + floatTowards.reTargetRate);
                    var point = floatTowards.random.NextFloat3( -targetArea.area / 2f, targetArea.area / 2f );
                    floatTowards.targetPoint = targetAreaTransform.TransformPoint( point );
                }

                // move towards point
                var direction = math.normalize( floatTowards.targetPoint - transform.Position );
                var moveImpulse = direction * floatTowards.speed;
                physicsVelocity.ApplyLinearImpulse( physicsMass, transform.Scale, moveImpulse );
            }
        }
    }
}