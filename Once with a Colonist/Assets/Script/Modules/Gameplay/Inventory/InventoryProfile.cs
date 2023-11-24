using JetBrains.Annotations;
using MemoryPack;
using UniRx;

namespace TendedTarsier
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class InventoryProfile : ProfileBase
    {
        public override string Name => "Inventory";

        [MemoryPackOrder(0)]
        public ReactiveDictionary<string, InventoryItemModel> ChangedTiles { get; [UsedImplicitly] set; } = new();
    }
}