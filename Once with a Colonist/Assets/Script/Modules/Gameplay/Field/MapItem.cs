using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Field
{
    public class MapItem : MonoBehaviour
    {
        [field: SerializeField]
        public ItemEntity ItemEntity { get; set; }
        
        [field: SerializeField]
        public Collider2D Collider { get; set; }
        [field: SerializeField]
        public SpriteRenderer SpriteRenderer { get; set; }
    }

}