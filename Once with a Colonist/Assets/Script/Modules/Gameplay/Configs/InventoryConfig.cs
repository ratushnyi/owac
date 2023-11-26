using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TendedTarsier
{
    [CreateAssetMenu(menuName = "InventoryConfig", fileName = "InventoryConfig")]
    public class InventoryConfig : ScriptableObject
    {
        [field: SerializeField]
        public InventoryCellView InventoryCellView { get; set; }
        
        [field: SerializeField]
        public Vector2Int InventoryGrid { get; set; }
        
        [field: SerializeField]
        private List<InventoryItemModel> InventoryItems { get; set; }
        
        public InventoryItemModel this[string id] => InventoryItems.FirstOrDefault(t => t.Id == id);
    }
}