using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Field
{
    public abstract class MapItemBase : MonoBehaviour
    {
        [field: SerializeField]
        public string Id { get; set; }

        [field: SerializeField]
        public int Count { get; set; }
        
        [field: SerializeField]
        public Collider2D Collider { get; set; }
        [field: SerializeField]
        public SpriteRenderer SpriteRenderer { get; set; }
    }

}