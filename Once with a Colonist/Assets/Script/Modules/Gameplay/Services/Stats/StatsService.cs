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
        }

        private void InitializeProfile()
        {
            foreach (var statEntity in _statsConfig.StatsList)
            {
                if (!_statsProfile.StatsDictionary.ContainsKey(statEntity.StatType))
                {
                    var levelEntity = statEntity.GetLevel(0);
                    _statsProfile.StatsDictionary.Add(statEntity.StatType, new StatProfileElement
                    {
                        Value = new ReactiveProperty<int>(levelEntity.DefaultValue),
                        Range = new ReactiveProperty<int>(levelEntity.Range)
                    });
                }
            }

            foreach (var stat in _statsProfile.StatsDictionary)
            {
                var statsModel = _statsConfig.GetStatsModel(stat.Key);
                stat.Value.Experience.Subscribe(_ => OnExperienceChanged(stat.Key, stat.Value)).AddTo(CompositeDisposable);

                if (statsModel.StatBar)
                {
                    _hudService.ShowStatBar(stat.Key, stat.Value.Value, stat.Value.Range);
                }
                if (statsModel.Observable)
                {
                    ObserveStat(stat.Key);
                }
            }
        }

        private void OnExperienceChanged(StatType statType, StatProfileElement statProfileElement)
        {
            var statsModel = _statsConfig.GetStatsModel(statType);

            var extraExperience = statProfileElement.Experience.Value - statsModel.GetLevel(statProfileElement.Level.Value).BorderValue;
            if (extraExperience >= 0)
            {
                statProfileElement.Experience.Value = extraExperience;
                statProfileElement.Level.Value++;

                var levelEntity = statsModel.GetLevel(statProfileElement.Level.Value);
                statProfileElement.Value.Value = levelEntity.DefaultValue;
                statProfileElement.Range.Value = levelEntity.Range;
            }
        }

        private void ObserveStat(StatType statType)
        {
            var profileElement = _statsProfile.StatsDictionary[statType];
            var levelModel = _statsConfig.GetStatsModel(statType).GetLevel(profileElement.Level.Value);

            Observable.Timer(TimeSpan.FromSeconds(levelModel.RecoveryRate)).Repeat()
                .Subscribe(_ => ApplyValue(statType, levelModel.RecoveryValue))
                .AddTo(CompositeDisposable);
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

        public bool IsSuitable(StatType statType, int value)
        {
            var profileElement = _statsProfile.StatsDictionary[statType];
            var levelModel = _statsConfig.GetStatsModel(statType).GetLevel(profileElement.Level.Value);
            var hypotheticalValue = profileElement.Value.Value + value;
            var newValue = Math.Min(levelModel.Range, profileElement.Value.Value + value);
            newValue = Math.Max(0, newValue);

            return hypotheticalValue == newValue;
        }

        public bool ApplyValue(StatType statType, int value)
        {
            var profileElement = _statsProfile.StatsDictionary[statType];
            var levelModel = _statsConfig.GetStatsModel(statType).GetLevel(profileElement.Level.Value);
            var newValue = Math.Min(levelModel.Range, profileElement.Value.Value + value);
            newValue = Math.Max(0, newValue);

            if (profileElement.Value.Value == newValue)
            {
                return false;
            }

            var experience = newValue - profileElement.Value.Value;
            profileElement.Value.Value = newValue;
            profileElement.Experience.Value += experience;

            return true;
        }
    }
}