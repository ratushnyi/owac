using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class MapObjectBase : MonoBehaviour
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