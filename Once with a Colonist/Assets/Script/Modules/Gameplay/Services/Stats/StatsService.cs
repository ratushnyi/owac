using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Configs.Gameplay;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.General.Services;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Stats
{
    [UsedImplicitly]
    public class StatsService : ServiceBase
    {
        private readonly HUDService _hudService;
        private readonly GameplayConfig _gameplayConfig;
        private readonly StatsConfig _statsConfig;
        private readonly StatsProfile _statsProfile;
        public float MovementSpeed => _gameplayConfig.MovementSpeed;
        public int DropDistance => _gameplayConfig.DropDistance;
        public Vector3 PlayerPosition => _statsProfile.PlayerPosition;

        public StatsService(
            HUDService hudService,
            GameplayConfig gameplayConfig,
            StatsConfig statsConfig,
            StatsProfile statsProfile)
        {
            _hudService = hudService;
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
                    var levelEntity = statEntity.GetLevel(0);
                    _statsProfile.StatsDictionary.Add(statEntity.StatType, new StatsProfileElement
                    {
                        Value = new ReactiveProperty<int>(levelEntity.DefaultValue),
                        Range = new ReactiveProperty<int>(levelEntity.Range)
                    });
                }
            }

            foreach (var stat in _statsProfile.StatsDictionary)
            {
                _hudService.ShowStatBar(stat.Key, stat.Value.Value, stat.Value.Range);
                stat.Value.Experience.Subscribe(_ => OnExperienceChanged(stat.Key, stat.Value)).AddTo(CompositeDisposable);
            }
        }

        private void OnExperienceChanged(StatType statType, StatsProfileElement statsProfileElement)
        {
            var statEntity = _statsConfig.GetStatsEntity(statType);

            var extraExperience = statsProfileElement.Experience.Value - statEntity.GetLevel(statsProfileElement.Level.Value).BorderValue;
            if (extraExperience >= 0)
            {
                statsProfileElement.Experience.Value = extraExperience;
                statsProfileElement.Level.Value++;

                var levelEntity = statEntity.GetLevel(statsProfileElement.Level.Value);
                statsProfileElement.Value.Value = levelEntity.DefaultValue;
                statsProfileElement.Range.Value = levelEntity.Range;
            }
        }

        private void ObserveEnergy()
        {
            var energyElement = _statsProfile.StatsDictionary[StatType.Energy];
            var statLevelEntity = _statsConfig.GetStatsEntity(StatType.Energy).GetLevel(energyElement.Level.Value);

            energyElement.Level.Subscribe(onLevelChanged)
                .AddTo(CompositeDisposable);

            Observable.Timer(TimeSpan.FromSeconds(statLevelEntity.RecoveryRate)).Repeat()
                .Subscribe(_ => onEnergyRecovered())
                .AddTo(CompositeDisposable);

            void onEnergyRecovered()
            {
                var newValue = Math.Min(statLevelEntity.Range, energyElement.Value.Value + 1);
                var experience = newValue - energyElement.Value.Value;
                energyElement.Value.Value = newValue;
                energyElement.Experience.Value += experience;
            }

            void onLevelChanged(int level)
            {
                statLevelEntity = _statsConfig.GetStatsEntity(StatType.Energy).GetLevel(level);
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