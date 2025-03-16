using System;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

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
        private readonly NetworkManager _networkManager;

        public PlayerService(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform mapItemsContainer,
            PlayerProfile playerProfile,
            PlayerConfig playerConfig,
            DiContainer container,
            NetworkManager networkManager)
        {
            _mapItemsContainer = mapItemsContainer;
            _playerProfile = playerProfile;
            _playerConfig = playerConfig;
            _container = container;
            _networkManager = networkManager;
        }

        protected override void Initialize()
        {
            PlayerController = Object.Instantiate(_playerConfig.PlayerPrefab, _mapItemsContainer);
            _container.Inject(PlayerController);

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
            _playerProfile.PlayerMapModel.Position = PlayerPosition.Value;
            _playerProfile.PlayerMapModel.SortingLayerID = PlayerSortingLayerID.Value;
            _playerProfile.PlayerMapModel.LayerID = PlayerLayerID.Value;
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }
    }
}