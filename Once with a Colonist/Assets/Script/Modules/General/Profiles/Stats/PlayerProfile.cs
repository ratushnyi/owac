using System;
using MemoryPack;
using TendedTarsier.Script.Modules.General.Profiles.Map;
using TendedTarsier.Script.Modules.General.Services.Profile;

namespace TendedTarsier.Script.Modules.General.Profiles.Stats
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class PlayerProfile : ProfileBase
    {
        [MemoryPackIgnore]
        public bool IsFirstStart => FirstStartDate == LastSaveDate;

        public override string Name => "Player";

        [MemoryPackOrder(0)]
        public DateTime? FirstStartDate { get; set; }

        [MemoryPackOrder(1)]
        public DateTime? LastSaveDate { get; set; }

        [MemoryPackOrder(2)]
        public ItemMapModel PlayerMapModel { get; set; } = new();
    }
}