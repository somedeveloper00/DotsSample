using Unity.Entities;
using UnityEngine;

namespace SampleGame.FloatTowards {

    public struct FloatTargetAreaTag : IComponentData { }

    public class FloatTargetAreaTagAuthoring : MonoBehaviour {
        class FloatTargetAreaTagBaker : Baker<FloatTargetAreaTagAuthoring> {
            public override void Bake(FloatTargetAreaTagAuthoring authoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                AddComponent( entity, new FloatTargetAreaTag() );
            }
        }
    }
}