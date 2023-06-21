using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace SampleGame {
    public class CustomAuthoringExample : MonoBehaviour {
        EntityManager _entityManager;
        Entity _entity;

        void OnEnable() {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // create entity
            _entity = _entityManager.CreateEntity();
            
            // populate entity with components
            var localTransform = LocalTransform.FromPositionRotationScale( 
                transform.position, transform.rotation, transform.localScale.x );
            _entityManager.AddComponentData( _entity, localTransform );
            // ...
        }

        void Update() {
            // use components 
            var localTransform = _entityManager.GetComponentData<LocalTransform>( _entity );
            // ...
        }
    }
}