using MemoryPack;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;

namespace TendedTarsier.Script.Modules.General.Profiles.Map
{
    [MemoryPackable]
    public partial class MapItemModel
    {
        [MemoryPackAllowSerialize]
        public Vector3 Position { get; set; }
        [MemoryPackAllowSerialize]
        public int SortingLayerID { get; set; }
        [MemoryPackAllowSerialize]
        public int LayerID { get; set; }

        [MemoryPackAllowSerialize]
        public ItemEntity ItemEntity { get; set; }
    }
}