using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SampleGame.Spawn {
    public class AreaAuthoring : MonoBehaviour {
        public Vector3 area;
         
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube( Vector3.zero, area );
        }

        class AreaBaker : Baker<AreaAuthoring> {
            public override void Bake(AreaAuthoring authoring) {
                var entity = GetEntity( TransformUsageFlags.Dynamic );
                // convert child space area to local space
                var scale = authoring.transform.localScale;
                var area = new float3( 
                    scale.x * authoring.area.x,
                    scale.y * authoring.area.y,
                    scale.z * authoring.area.z );
                AddComponent( entity, new AreaComponentData {
                    area = area
                } );
            }
        }
    }
}