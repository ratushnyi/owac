using System;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Player
{
    [UsedImplicitly]
    public class PlayerService : ServiceBase
    {
        public Vector3Int TargetPosition => new Vector3Int(Mathf.FloorToInt(PlayerPosition.Value.x), Mathf.RoundToInt(PlayerPosition.Value.y)) + TargetDirection.Value;
        public PlayerController PlayerController { get; private set; }
        public readonly ReactiveProperty<Vector3Int> TargetDirection = new();
        public readonly ReactiveProperty<Vector3> PlayerPosition = new();
        public readonly ReactiveProperty<int> PlayerSortingLayerID = new();
        public readonly ReactiveProperty<int> PlayerLayerID = new();

        private readonly Transform _mapItemsContainer;
        private readonly PlayerProfile _playerProfile;
        private readonly PlayerConfig _playerConfig;
        private readonly DiContainer _container;

        public PlayerService(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform mapItemsContainer,
            PlayerProfile playerProfile,
            PlayerConfig playerConfig,
            DiContainer container)
        {
            _mapItemsContainer = mapItemsContainer;
            _playerProfile = playerProfile;
            _playerConfig = playerConfig;
            _container = container;
        }

        protected override void Initialize()
        {
            PlayerController = _container.InstantiatePrefabForComponent<PlayerController>(_playerConfig.PlayerPrefab, _mapItemsContainer);
            if (!_playerProfile.IsFirstStart)
            {
                PlayerController.transform.SetLocalPositionAndRotation(_playerProfile.PlayerMapModel.Position, Quaternion.identity);
                PlayerController.ApplyLayer(_playerProfile.PlayerMapModel.LayerID, _playerProfile.PlayerMapModel.SortingLayerID);
            }

            OnSessionStarted();
        }

        public void OnSessionStarted()
        {
            _playerProfile.FirstStartDate ??= DateTime.UtcNow;
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }

        public override void Dispose()
        {
            base.Dispose();
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }

        public void UpdatePlayerMapModel()
        {
            _playerProfile.PlayerMapModel.Position = PlayerPosition.Value;
            _playerProfile.PlayerMapModel.SortingLayerID = PlayerSortingLayerID.Value;
            _playerProfile.PlayerMapModel.LayerID = PlayerLayerID.Value;
        }
    }
}