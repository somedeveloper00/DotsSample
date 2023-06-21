using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace SampleGame.Spawn {
    public partial struct SpawnSystem : ISystem {
        [BurstCompile] public void OnCreate(ref SystemState state) => 
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

        [BurstCompile] public void OnUpdate(ref SystemState state) {
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer( state.WorldUnmanaged );
            
            foreach (var (transform, spawn, area) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<SpawnComponentData>, RefRO<AreaComponentData>>()) {
                if (SystemAPI.Time.ElapsedTime > spawn.ValueRW.nextSpawnTime) {
                    spawn.ValueRW.nextSpawnTime = (float)(SystemAPI.Time.ElapsedTime + spawn.ValueRW.spawnRate);

                    for (int i = 0; i < spawn.ValueRO.spawnCount; i++) {
                        var entity = ecb.Instantiate( spawn.ValueRW.prefab );
                        var randomPoint = spawn.ValueRW.random.NextFloat3( -area.ValueRO.area / 2f, area.ValueRO.area / 2f );
                        var pos = transform.ValueRW.TransformPoint( randomPoint ); // local to world space
                        var localTransformComponent = LocalTransform.FromPosition( pos );
                        ecb.AddComponent( entity, localTransformComponent );
                    }
                }
            }
        }
    }
}