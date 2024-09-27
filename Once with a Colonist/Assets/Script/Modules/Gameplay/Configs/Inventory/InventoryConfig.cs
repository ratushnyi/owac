using System.Collections.Generic;
using System.Linq;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapItem;
using UnityEngine;
using Zenject;
using ItemModel = TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items.ItemModel;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Inventory
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
        private List<ItemModel> InventoryItems { get; set; }

        [field: SerializeField]
        public MapItem MapItemPrefab { get; set; }

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            InventoryItems.ForEach(t => diContainer.Inject(t.Tool));
        }
    }
}