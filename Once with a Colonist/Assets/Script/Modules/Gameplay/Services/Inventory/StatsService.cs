using System;
using UnityEngine;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;

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
            ObserveEnergy();
        }

        private void ObserveEnergy()
        {
            var energyLevel = _statsProfile.StatsDictionary[StatType.EnergyLevel];
            var energyValue = _statsProfile.StatsDictionary[StatType.Energy];
            var statLevelEntity = _statsConfig.GetStatsEntity(StatType.Energy).Levels[energyLevel.Value];

            Observable.Timer(TimeSpan.FromSeconds(statLevelEntity.RecoveryRate)).Repeat()
                .Subscribe(_ => onEnergyRecovered())
                .AddTo(CompositeDisposable);

            void onEnergyRecovered()
            {
                energyValue.Value = Math.Min(statLevelEntity.BorderValue, energyValue.Value + 1);
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