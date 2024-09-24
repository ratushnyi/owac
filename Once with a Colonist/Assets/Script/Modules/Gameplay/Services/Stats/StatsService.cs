using System;
using UnityEngine;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Configs.Gameplay;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using StatsProfile = TendedTarsier.Script.Modules.General.Profiles.Stats.StatsProfile;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory
{
    [UsedImplicitly]
    public class StatsService : ServiceBase
    {
        private readonly GameplayConfig _gameplayConfig;
        private readonly StatsConfig _statsConfig;
        private readonly StatsProfile _statsProfile;
        public float MovementSpeed => _gameplayConfig.MovementSpeed;
        public int DropDistance => _gameplayConfig.DropDistance;
        public Vector3 PlayerPosition => _statsProfile.PlayerPosition;

        public StatsService(
            GameplayConfig gameplayConfig,
            StatsConfig statsConfig,
            StatsProfile statsProfile)
        {
            _gameplayConfig = gameplayConfig;
            _statsConfig = statsConfig;
            _statsProfile = statsProfile;
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitializeProfile();
            ObserveEnergy();
        }

        private void InitializeProfile()
        {
            foreach (var statEntity in _statsConfig.StatsList)
            {
                if (!_statsProfile.StatsDictionary.ContainsKey(statEntity.StatType))
                {
                    _statsProfile.StatsDictionary.Add(statEntity.StatType, new ReactiveProperty<StatsProfileElement>(new StatsProfileElement{Value = statEntity.Levels[0].Range}));
                }
            }
        }

        private void ObserveEnergy()
        {
            var energyElement = _statsProfile.StatsDictionary[StatType.Energy];
            var statLevelEntity = _statsConfig.GetStatsEntity(StatType.Energy).Levels[energyElement.Value.Level];

            Observable.Timer(TimeSpan.FromSeconds(statLevelEntity.RecoveryRate)).Repeat()
                .Subscribe(_ => onEnergyRecovered())
                .AddTo(CompositeDisposable);

            void onEnergyRecovered()
            {
                var newValue = Math.Min(statLevelEntity.BorderValue, energyElement.Value.Value + 1);
                energyElement.Value.Experience += newValue - energyElement.Value.Value;
                energyElement.Value.Value = newValue;
            }
        }

        public void OnSessionStarted()
        {
            _statsProfile.StartDate ??= DateTime.UtcNow;
            _statsProfile.LastSaveDate = DateTime.UtcNow;
            _statsProfile.Save();
        }

        public void OnSessionEnded(Vector2 position)
        {
            _statsProfile.PlayerPosition = position;
            _statsProfile.LastSaveDate = DateTime.UtcNow;
            _statsProfile.Save();
        }
    }
}