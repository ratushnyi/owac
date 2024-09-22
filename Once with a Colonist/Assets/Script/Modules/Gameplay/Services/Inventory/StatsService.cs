using System;
using UnityEngine;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.General.Services;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory
{
    [UsedImplicitly]
    public class StatsService : ServiceBase
    {
        private readonly GameplayConfig _gameplayConfig;
        private readonly PlayerProfile _playerProfile;
        public float MovementSpeed => _gameplayConfig.MovementSpeed;
        public int DropDistance => _gameplayConfig.DropDistance;
        public Vector3 PlayerPosition => _playerProfile.PlayerPosition;

        public StatsService(GameplayConfig gameplayConfig, PlayerProfile playerProfile)
        {
            _gameplayConfig = gameplayConfig;
            _playerProfile = playerProfile;
        }
        
        public void OnSessionStarted()
        {
            _playerProfile.StartDate ??= DateTime.UtcNow;
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }
        
        public void OnSessionEnded(Vector2 position)
        {
            _playerProfile.PlayerPosition = position;
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }
    }
}