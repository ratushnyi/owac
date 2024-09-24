using MemoryPack;
using UniRx;

namespace TendedTarsier.Script.Modules.General.Profiles.Stats
{
    [MemoryPackable]
    public partial class StatsProfileElement
    {
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> Level;
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> Experience;
        [MemoryPackAllowSerialize]
        public ReactiveProperty<int> CurrentValue;
    }
}