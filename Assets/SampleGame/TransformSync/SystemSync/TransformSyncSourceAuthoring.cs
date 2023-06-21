using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame {

    public struct TransformSyncSource : IComponentData {
        public FixedString32Bytes id;
    }

    public class TransformSyncSourceAuthoring : MonoBehaviour {
        public string id;
        
        class TargetTransformBaker : Baker<TransformSyncSourceAuthoring> {
            public override void Bake(TransformSyncSourceAuthoring syncSourceAuthoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                AddComponent( entity, new TransformSyncSource {
                    id = syncSourceAuthoring.id
                } );
            }
        }
    }

    public partial struct SyncTransformSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (constraint, transform) in SystemAPI.Query<
                         RefRO<TransformSyncSource>, RefRO<LocalTransform>>()) 
            {
                for (int i = 0; i < SyncTransformToEntity.Instances.Count; i++) {
                    if (SyncTransformToEntity.Instances[i].id == constraint.ValueRO.id) {
                        if (SyncTransformToEntity.Instances[i].position)
                            SyncTransformToEntity.Instances[i].transform.position = transform.ValueRO.Position;
                        if (SyncTransformToEntity.Instances[i].rotation)
                            SyncTransformToEntity.Instances[i].transform.rotation = transform.ValueRO.Rotation;
                    }
                }
            }
        }
    }
}