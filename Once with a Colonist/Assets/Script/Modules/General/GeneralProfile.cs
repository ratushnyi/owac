using System;
using MemoryPack;

namespace TendedTarsier
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GeneralProfile : ProfileSection
    {
        public override string Name => "Gameplay";

        [MemoryPackOrder(0)]
        public DateTime FirstOpen { get; set; }

    }
}