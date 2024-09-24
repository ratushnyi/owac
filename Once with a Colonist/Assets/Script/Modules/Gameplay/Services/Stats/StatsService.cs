using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Configs.Gameplay;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Stats
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
                    _statsProfile.StatsDictionary.Add(statEntity.StatType, new StatsProfileElement { CurrentValue = new ReactiveProperty<int>(statEntity.GetLevel(0).Range)});
                }
            }

            foreach (var stat in _statsProfile.StatsDictionary)
            {
                stat.Value.Experience.Subscribe(_ => OnExperienceIncreased(stat.Key, stat.Value)).AddTo(CompositeDisposable);
            }
        }

        private void OnExperienceIncreased(StatType statType, StatsProfileElement statsProfileElement)
        {
            var statEntity = _statsConfig.GetStatsEntity(statType);

            var extraExperience = statsProfileElement.Experience.Value - statEntity.GetLevel(statsProfileElement.Level.Value).BorderValue;
            if (extraExperience >= 0)
            {
                statsProfileElement.Level.Value++;
                statsProfileElement.Experience.Value = extraExperience;
            }
        }

        private void ObserveEnergy()
        {
            var energyElement = _statsProfile.StatsDictionary[StatType.Energy];
            var statLevelEntity = _statsConfig.GetStatsEntity(StatType.Energy).GetLevel(energyElement.Level.Value);

            Observable.Timer(TimeSpan.FromSeconds(statLevelEntity.RecoveryRate)).Repeat()
                .Subscribe(_ => onEnergyRecovered())
                .AddTo(CompositeDisposable);

            void onEnergyRecovered()
            {
                var newValue = Math.Min(statLevelEntity.BorderValue, energyElement.CurrentValue.Value + 1);
                energyElement.Experience.Value += newValue - energyElement.CurrentValue.Value;
                energyElement.CurrentValue.Value = newValue;
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