using Unity.Netcode;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class MapObjectBase : NetworkBehaviour
    {
        [field: SerializeField]
        public Collider2D Collider { get; set; }
        [field: SerializeField]
        public SpriteRenderer SpriteRenderer { get; set; }
        
        private void Start()
        {
            Collider.isTrigger = true;
        }
    }
}