using Unity.Entities;
using UnityEngine;

namespace SampleGame.Player {
    public struct PlayerComponentData : IComponentData {
        public float someValue;
    }


    public class Player : MonoBehaviour {
        EntityManager _entityManager;
        Entity _entity;

        void OnEnable() {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData( _entity, new PlayerComponentData() );
        }

        [ContextMenu( "Use Data" )]
        public void UseData() {
            var data = _entityManager.GetComponentData<PlayerComponentData>( _entity );
            Debug.Log( data.someValue );
        }
    }
}