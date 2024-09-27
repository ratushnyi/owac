using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryPack;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using TendedTarsier.Script.Modules.General.Services.Profile;

namespace TendedTarsier.Script.Modules.General.Profiles.Stats
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class StatsProfile : ProfileBase
    {
        public override string Name => "Stats";

        [MemoryPackOrder(0)]
        public Dictionary<StatType, StatProfileElement> StatsDictionary { get; [UsedImplicitly] set; } = new();
    }
}