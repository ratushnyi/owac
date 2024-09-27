using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class MapObjectBase : MonoBehaviour
    {
        [field: SerializeField]
        public Collider2D Collider { get; set; }

        [field: SerializeField]
        public SpriteRenderer SpriteRenderer { get; set; }
    }
}