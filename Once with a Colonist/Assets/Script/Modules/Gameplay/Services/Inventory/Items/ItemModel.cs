using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    [Serializable]
    public class ItemModel
    {
        [field: SerializeField]
        public string Id { get; set; }

        [field: SerializeField]
        public Sprite Sprite { get; set; }

        [field: SerializeField]
        public ToolEntityBase ToolEntity { get; set; }

        public bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            return ToolEntity != null && ToolEntity.Perform(tilemap, targetPosition);
        }
    }
}