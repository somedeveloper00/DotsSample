using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace SampleGame {
    
    public struct FreezeRotationComponentData : IComponentData {
        public bool3 Flags;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class FreezeRotationAuthoring : MonoBehaviour {
        public bool3 Flags;


        class FreezeRotationBaker : Baker<FreezeRotationAuthoring> {
            public override void Bake(FreezeRotationAuthoring authoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                AddComponent( entity, new FreezeRotationComponentData {
                    Flags = authoring.Flags
                } );
            }
        }
    }

    public partial struct FreezeRotationSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (freeRot, pmass) in SystemAPI.Query<
                         RefRO<FreezeRotationComponentData>, RefRW<PhysicsMass>>()) {
                if (freeRot.ValueRO.Flags.x) pmass.ValueRW.InverseInertia.x = 0f;
                if (freeRot.ValueRO.Flags.y) pmass.ValueRW.InverseInertia.y = 0f;
                if (freeRot.ValueRO.Flags.z) pmass.ValueRW.InverseInertia.z = 0f;
            }
        }
    }
}