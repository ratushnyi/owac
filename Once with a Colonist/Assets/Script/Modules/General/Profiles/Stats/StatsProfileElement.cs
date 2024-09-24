using MemoryPack;
using UniRx;

namespace TendedTarsier.Script.Modules.General.Profiles.Stats
{
    [MemoryPackable]
    public partial class StatsProfileElement
    {
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> Level = new();
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> Experience = new();
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> Value = new();
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> Range = new();
    }
}