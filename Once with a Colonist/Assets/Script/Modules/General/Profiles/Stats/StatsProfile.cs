using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryPack;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using TendedTarsier.Script.Modules.General.Services.Profile;

namespace TendedTarsier.Script.Modules.General.Profiles.Stats
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class StatsProfile : ProfileBase
    {
        private StatsConfig _config;
        public override string Name => "Stats";

        [MemoryPackOrder(0)]
        public DateTime? StartDate { get; set; }

        [MemoryPackOrder(1)]
        public DateTime? LastSaveDate { get; set; }

        [MemoryPackOrder(2)]
        public Vector2 PlayerPosition { get; set; }

        [MemoryPackOrder(3)]
        public int SoringLayerID { get; set; }

        [MemoryPackOrder(4)]
        public int Layer { get; set; }

        [MemoryPackOrder(5)]
        public Dictionary<StatType, StatProfileElement> StatsDictionary { get; [UsedImplicitly] set; } = new();
    }
}