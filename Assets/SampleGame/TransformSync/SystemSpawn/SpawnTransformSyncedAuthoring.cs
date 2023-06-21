using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame {
    public class SpawnGameObjectSynced : IComponentData {
        public Transform instance;
        public bool spawned;
        public bool position;
        public bool rotation;
    }

    public class SpawnTransformSyncedAuthoring : MonoBehaviour {
        public Transform prefab;
        public bool position;
        public bool rotation;


        class SpawnGameObjectSyncedBaker : Baker<SpawnTransformSyncedAuthoring> {
            public override void Bake(SpawnTransformSyncedAuthoring authoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                AddComponentObject( entity, new SpawnGameObjectSynced {
                    instance = authoring.prefab,
                    position = authoring.position,
                    rotation = authoring.rotation
                } );
            }
        }
    }

    public partial struct SpawnGameObjectSyncedSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (spawn, transform) in SystemAPI.Query<
                         SpawnGameObjectSynced, RefRO<LocalTransform>>()) 
            {
                if (spawn.instance != null) {
                    // spawn
                    if (!spawn.spawned) {
                        spawn.instance = Object.Instantiate( spawn.instance );
                        spawn.spawned = true;
                    }
                    // sync
                    if (spawn.position)
                        spawn.instance.transform.position = transform.ValueRO.Position;
                    if (spawn.rotation)
                        spawn.instance.transform.rotation = transform.ValueRO.Rotation;
                }
            }
        }
    }
}