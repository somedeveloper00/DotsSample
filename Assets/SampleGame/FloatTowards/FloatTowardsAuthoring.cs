using SampleGame.Spawn;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace SampleGame.FloatTowards {
    
    public struct FloatTowardsComponentData : IComponentData {
        public float speed;
        public float reTargetRate;
        public float3 targetPoint;
        public float nextReTargetTime;
        public Random random;
    }

    public class FloatTowardsAuthoring : MonoBehaviour {
        public float speed;
        public float reTargetRate;
        
        class FloatTowardsBaker : Baker<FloatTowardsAuthoring> {
            public override void Bake(FloatTowardsAuthoring authoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                AddComponent( entity,
                    new FloatTowardsComponentData {
                        speed = authoring.speed,
                        reTargetRate = authoring.reTargetRate,
                        random = new Random( (uint)new System.Random().Next() )
                    } );
            }
        }
    }
}