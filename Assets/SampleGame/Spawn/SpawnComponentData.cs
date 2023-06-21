using System;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace SampleGame.Spawn {
    public struct SpawnComponentData : IComponentData {
        public Entity prefab;
        public float spawnRate;
        public int spawnCount;
        public float nextSpawnTime;
        public Random random;
    }

    [Serializable] 
    public struct AreaComponentData : IComponentData {
        public float3 area;
    }
}