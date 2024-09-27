using MemoryPack;
using UnityEngine;
using ItemEntity = TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items.ItemEntity;

namespace TendedTarsier.Script.Modules.General.Profiles.Tilemap
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