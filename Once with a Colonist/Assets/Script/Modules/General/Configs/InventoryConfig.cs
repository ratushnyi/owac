using System.Collections.Generic;
using System.Linq;
using TendedTarsier.Script.Modules.Gameplay.Panels.Inventory;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;

namespace TendedTarsier.Script.Modules.General.Configs
{
    [CreateAssetMenu(menuName = "Config/InventoryConfig", fileName = "InventoryConfig")]
    public class InventoryConfig : ScriptableObject
    {
        public ItemModel this[string id] => InventoryItems.FirstOrDefault(t => t.Id == id);

        [field: SerializeField]
        public InventoryCellView InventoryCellView { get; set; }

        [field: SerializeField]
        public int InventoryCapacity { get; set; }

        [field: SerializeField]
        public List<ItemModel> InventoryItems { get; set; }
    }
}