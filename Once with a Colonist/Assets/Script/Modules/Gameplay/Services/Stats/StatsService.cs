using System;
using System.Collections.Generic;
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

        private readonly Dictionary<StatType, IDisposable> _feeDisposables = new();

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
            InitializeStats();
            InitializeFees();
        }

        private void InitializeProfile()
        {
            foreach (var statEntity in _statsConfig.StatsList)
            {
                if (!_statsProfile.StatsDictionary.ContainsKey(statEntity.StatType))
                {
                    var levelEntity = statEntity[0];
                    _statsProfile.StatsDictionary.Add(statEntity.StatType, new StatProfileElement
                    {
                        Value = new ReactiveProperty<int>(levelEntity.DefaultValue),
                        Range = new ReactiveProperty<int>(levelEntity.MaxValue)
                    });
                }
            }
        }

        private void InitializeStats()
        {
            void onExperienceValueChanged(StatType statType, StatProfileElement statProfileElement)
            {
                var statsModel = _statsConfig[statType];

                var extraExperience = statProfileElement.Experience.Value - statsModel[statProfileElement.Level.Value].NextLevelExperience;
                if (extraExperience >= 0)
                {
                    statProfileElement.Experience.Value = extraExperience;
                    statProfileElement.Level.Value++;

                    var levelEntity = statsModel[statProfileElement.Level.Value];
                    statProfileElement.Value.Value = levelEntity.DefaultValue;
                    statProfileElement.Range.Value = levelEntity.MaxValue;
                }
            }

            void observeStat(StatType statType)
            {
                var profileElement = _statsProfile.StatsDictionary[statType];
                var levelModel = _statsConfig[statType][profileElement.Level.Value];

                StartApplyValueAutoApplyValue(statType, levelModel.RecoveryRate, levelModel.RecoveryValue);
            }

            foreach (var stat in _statsProfile.StatsDictionary)
            {
                var statsModel = _statsConfig[stat.Key];

                stat.Value.Experience
                    .Subscribe(_ => onExperienceValueChanged(stat.Key, stat.Value))
                    .AddTo(CompositeDisposable);

                if (statsModel.StatBar)
                {
                    _hudService.ShowStatBar(stat.Key, statsModel, stat.Value);
                }
                if (statsModel.Observable)
                {
                    observeStat(stat.Key);
                }
            }
        }

        private void InitializeFees()
        {
            foreach (var stat in _statsConfig.StatsFeeConditionalList)
            {
                var profileElement = _statsProfile.StatsDictionary[stat.Type];
                profileElement.Value.Subscribe(t => onFeeStatChanged(stat, profileElement, t)).AddTo(CompositeDisposable);
            }

            void onFeeStatChanged(StatFeeConditionModel condition, StatProfileElement statProfileElement, int currentValue)
            {
                switch (condition.Condition)
                {
                    case StatFeeConditionModel.FeeConditionType.MaxValue:
                        var levelModel = _statsConfig[condition.Type][statProfileElement.Level.Value];
                        if (currentValue == levelModel.MaxValue)
                        {
                            var disposable = StartApplyValueAutoApplyValue(condition.FeeModel.Type, condition.FeeModel.Rate, condition.FeeModel.Value);

                            _feeDisposables.Add(condition.Type, disposable);
                        }
                        else
                        {
                            if (_feeDisposables.TryGetValue(condition.Type, out var disposable))
                            {
                                disposable.Dispose();
                                _feeDisposables.Remove(condition.Type);
                            }
                        }
                        break;
                    case StatFeeConditionModel.FeeConditionType.MinValue:
                        if (currentValue == 0)
                        {
                            var disposable = StartApplyValueAutoApplyValue(condition.FeeModel.Type, condition.FeeModel.Rate, condition.FeeModel.Value);

                            _feeDisposables.Add(condition.Type, disposable);
                        }
                        else
                        {
                            if (_feeDisposables.TryGetValue(condition.Type, out var disposable))
                            {
                                disposable.Dispose();
                                _feeDisposables.Remove(condition.Type);
                            }
                        }
                        break;
                }
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

        public bool IsSuitable(StatType statType, int value)
        {
            var profileElement = _statsProfile.StatsDictionary[statType];
            var levelModel = _statsConfig[statType][profileElement.Level.Value];
            var hypotheticalValue = profileElement.Value.Value + value;
            var newValue = Math.Min(levelModel.MaxValue, profileElement.Value.Value + value);
            newValue = Math.Max(0, newValue);

            return hypotheticalValue == newValue;
        }

        public bool ApplyValue(StatType statType, int value)
        {
            var profileElement = _statsProfile.StatsDictionary[statType];
            var levelModel = _statsConfig[statType][profileElement.Level.Value];
            var newValue = Math.Min(levelModel.MaxValue, profileElement.Value.Value + value);
            newValue = Math.Max(0, newValue);

            if (profileElement.Value.Value == newValue)
            {
                return false;
            }

            var experience = newValue - profileElement.Value.Value;
            profileElement.Value.Value = newValue;
            profileElement.Experience.Value += experience;

            _statsProfile.Save();

            return true;
        }

        private IDisposable StartApplyValueAutoApplyValue(StatType type, int rate, int value)
        {
            return Observable.Timer(TimeSpan.FromSeconds(rate)).Repeat()
                .Subscribe(_ => ApplyValue(type, value))
                .AddTo(CompositeDisposable);
        }
    }
}