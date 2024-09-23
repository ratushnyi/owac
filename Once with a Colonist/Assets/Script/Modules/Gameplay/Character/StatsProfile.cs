using System;
using JetBrains.Annotations;
using MemoryPack;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using UnityEngine;
using UniRx;
using TendedTarsier.Script.Modules.General.Services.Profile;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Character
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
        public ReactiveDictionary<StatType, ReactiveProperty<int>> StatsDictionary { get; [UsedImplicitly] set; } = new();

        [Inject]
        public void Construct(StatsConfig config)
        {
            _config = config;
        }

        public override void OnSectionLoaded()
        {
            foreach (var statEntity in _config.StatsList)
            {
                if (!StatsDictionary.ContainsKey(statEntity.StatType))
                {
                    StatsDictionary.Add(statEntity.StatType, new ReactiveProperty<int>(statEntity.Levels[0].BorderValue));
                }
            }
        }

        public override void OnSectionCreated()
        {
            foreach (var statEntity in _config.StatsList)
            {
                StatsDictionary.Add(statEntity.StatType, new ReactiveProperty<int>(statEntity.Levels[0].BorderValue));
            }
        }
    }
}