using JetBrains.Annotations;
using MemoryPack;
using UniRx;
using TendedTarsier.Script.Modules.General.Services.Profile;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class InventoryProfile : ProfileBase
    {
        public override string Name => "Inventory";

        [MemoryPackOrder(0)]
        public ReactiveDictionary<string, ReactiveProperty<int>> InventoryItems { get; [UsedImplicitly] set; } = new();

        [MemoryPackOrder(1), MemoryPackAllowSerialize]
        public ReactiveProperty<string> SelectedItem { get; [UsedImplicitly] set; } = new(string.Empty);
    }
}