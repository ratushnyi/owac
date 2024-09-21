using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Inventory.Items
{
    [Serializable]
    public class ItemModel
    {
        [field: SerializeField]
        public string Id { get; set; }
        [field: SerializeField]
        public Sprite Sprite { get; set; }
        [field: SerializeField]
        public PerformEntityBase PerformEntity { get; set; }

        public bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            return PerformEntity != null && PerformEntity.Perform(tilemap, targetPosition);
        }
    }

}