using System;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Tools;

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

        [field: SerializeField]
        public bool IsCountable { get; set; }

        public bool Perform()
        {
            return Tool != null && Tool.Perform();
        }
    }
}