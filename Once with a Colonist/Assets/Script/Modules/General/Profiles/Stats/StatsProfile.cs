using System;
using JetBrains.Annotations;
using MemoryPack;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
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
        public ReactiveDictionary<StatType, ReactiveProperty<StatsProfileElement>> StatsDictionary { get; [UsedImplicitly] set; } = new();
    }
}