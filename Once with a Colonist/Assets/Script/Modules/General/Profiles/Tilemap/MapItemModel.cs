using MemoryPack;
using TendedTarsier.Script.Modules.Gameplay.Field;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Profiles.Tilemap
{
    [MemoryPackable]
    public partial class MapItemModel
    {
        [MemoryPackAllowSerialize]
        public Vector3 Position { get; set; }

        [MemoryPackAllowSerialize]
        public ItemEntity ItemEntity { get; set; }
    }
}