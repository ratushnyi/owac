using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using Unity.Netcode;
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
        private readonly DiContainer _container;
        private readonly NetworkManager _networkManager;

        public PlayerService(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform mapItemsContainer,
            PlayerProfile playerProfile,
            DiContainer container,
            NetworkManager networkManager)
        {
            _mapItemsContainer = mapItemsContainer;
            _playerProfile = playerProfile;
            _container = container;
            _networkManager = networkManager;
        }

        public override void Initialize()
        {
            OnSessionStarted().Forget();
        }

        private async UniTask InitializePlayer()
        {
            await UniTask.WaitUntil(() => _networkManager.IsConnectedClient);
            var playerObject = _networkManager.LocalClient.PlayerObject;
            playerObject.TrySetParent(_mapItemsContainer);
            PlayerController = playerObject.GetComponent<PlayerController>();
            _container.Inject(PlayerController);
            PlayerController.Initialize();
        }

        private async UniTaskVoid OnSessionStarted()
        {
            await InitializePlayer();
            _playerProfile.FirstStartDate ??= DateTime.UtcNow;
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }

        public override void Dispose()
        {
            _playerProfile.PlayerMapModel.Position = PlayerPosition.Value;
            _playerProfile.PlayerMapModel.SortingLayerID = PlayerSortingLayerID.Value;
            _playerProfile.PlayerMapModel.LayerID = PlayerLayerID.Value;
            _playerProfile.LastSaveDate = DateTime.UtcNow;
            _playerProfile.Save();
        }
    }
}