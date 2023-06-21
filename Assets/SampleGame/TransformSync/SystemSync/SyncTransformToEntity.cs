using System.Collections.Generic;
using UnityEngine;

namespace SampleGame {
    public class SyncTransformToEntity : MonoBehaviour {
        public string id;
        public bool position = true;
        public bool rotation = true;

        public static readonly List<SyncTransformToEntity> Instances = new();

        void Awake() => Instances.Add( this );
        void OnDisable() => Instances.Remove( this );
    }
}