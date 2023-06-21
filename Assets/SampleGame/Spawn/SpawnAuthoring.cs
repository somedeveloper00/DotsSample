using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SampleGame.Spawn {
    
    [RequireComponent(typeof(AreaAuthoring))]
    public class SpawnAuthoring : MonoBehaviour {
        public GameObject prefab;
        public float spawnRate;
        public int spawnCount;


        class SpawnBaker : Baker<SpawnAuthoring> {
            public override void Bake(SpawnAuthoring authoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                AddComponent( entity, new SpawnComponentData {
                    prefab = GetEntity( authoring.prefab, TransformUsageFlags.Dynamic ),
                    spawnRate = authoring.spawnRate,
                    random = new Random( (uint)new System.Random().Next() ),
                    spawnCount = authoring.spawnCount,
                    nextSpawnTime = 0
                } );
            }
        }
    }
    
}