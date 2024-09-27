using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class ItemMapObject : MapObjectBase
    {
        [field: SerializeField]
        public ItemEntity ItemEntity { get; set; }
    }
}