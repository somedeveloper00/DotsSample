using Unity.Entities;

namespace SampleGame {
    public struct MovingComponentData : IComponentData {
        public float moveSpeed;
        public float rotateSpeed;
    }
}