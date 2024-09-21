using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Inventory.Items;

namespace TendedTarsier.Script.Modules.Gameplay.Configs
{
    [CreateAssetMenu(menuName = "Config/InventoryConfig", fileName = "InventoryConfig")]
    public class InventoryConfig : ScriptableObject
    {
        [field: SerializeField]
        public InventoryCellView InventoryCellView { get; set; }
        
        [field: SerializeField]
        public Vector2Int InventoryGrid { get; set; }
        
        [field: SerializeField]
        private List<ItemModel> InventoryItems { get; set; }
        [field: SerializeField]
        public MapItemBase MapItemPrefab { get; set; }
        
        public ItemModel this[string id] => InventoryItems.FirstOrDefault(t => t.Id == id);

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            InventoryItems.ForEach(t => diContainer.Inject(t.PerformEntity));
        }
    }
}