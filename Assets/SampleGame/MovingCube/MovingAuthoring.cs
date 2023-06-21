using Unity.Entities;
using UnityEngine;

namespace SampleGame {
    [DisallowMultipleComponent]
    [AddComponentMenu("SampleGame/Moving")]
    public class MovingAuthoring : MonoBehaviour {
        public float moveSpeed;
        public float rotateSpeed;
        
        class MovingBaker : Baker<MovingAuthoring> {
            public override void Bake(MovingAuthoring authoring) {
                // gets the entity associated with the authoring's GameObject
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                // creates a new component for the entity, and sets its values
                AddComponent( entity, new MovingComponentData {
                    moveSpeed = authoring.moveSpeed,
                    rotateSpeed = authoring.rotateSpeed
                } );
            }
        }
    }
}