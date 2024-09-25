using System;
using UnityEngine;

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
        public ToolBase Tool { get; set; }

        public bool Perform(Vector3Int targetPosition)
        {
            return Tool != null && Tool.Perform(targetPosition);
        }
    }
}