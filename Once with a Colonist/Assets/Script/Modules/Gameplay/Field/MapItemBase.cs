using UnityEngine;

namespace TendedTarsier
{
    public abstract class MapItemBase : MonoBehaviour
    {
        [field: SerializeField]
        public string Id { get; set; }

        [field: SerializeField]
        public int Count { get; set; }
        
        [field: SerializeField]
        public Collider2D Collider { get; set; }
    }

}