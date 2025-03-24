using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.Gameplay.Services.Player;
using TendedTarsier.Script.Modules.General.Profiles.Map;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Configs;
using Unity.Netcode;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map
{
    [UsedImplicitly]
    public class MapService : ServiceBase
    {
        private readonly NetworkManager _networkManager;
        private readonly TilemapService _tilemapService;
        private readonly PlayerService _playerService;
        private readonly MapProfile _mapProfile;
        private readonly MapConfig _mapConfig;

        private MapService(
            MapConfig mapConfig,
            MapProfile mapProfile,
            PlayerService playerService,
            TilemapService tilemapService,
            NetworkManager networkManager)
        {
            _mapConfig = mapConfig;
            _mapProfile = mapProfile;
            _playerService = playerService;
            _tilemapService = tilemapService;
            _networkManager = networkManager;
        }

        public override void Initialize()
        {
            if (!_networkManager.IsServer)
            {
                return;
            }
            InitMapItems();
        }

        private void InitMapItems()
        {
            foreach (var mapItem in _mapProfile.MapItemsList)
            {
                var networkItem = _networkManager.SpawnManager.InstantiateAndSpawn(_mapConfig.ItemMapObjectPrefab);
                var item = networkItem.GetComponent<ItemMapObject>();
                item.Setup(mapItem, mapItem.Position);
            }
        }

        public void DropMapItem(ItemEntity itemEntity, Vector3 emitterPosition, Vector3 targetPosition)
        {
            var tilemap = _tilemapService.GetTilemap(new Vector2Int(Mathf.CeilToInt(targetPosition.x), Mathf.CeilToInt(targetPosition.y)));
            if (tilemap == null)
            {
                return;
            }

            var itemMapModel = new ItemMapModel
            {
                SortingLayerID = SortingLayer.NameToID(tilemap.GetComponent<Renderer>().sortingLayerName),
                LayerID = tilemap.gameObject.layer,
                ItemEntity = itemEntity,
                Position = targetPosition
            };

            _mapProfile.MapItemsList.Add(itemMapModel);
            _mapProfile.Save();

            var networkItem = _networkManager.SpawnManager.InstantiateAndSpawn(_mapConfig.ItemMapObjectPrefab);
            var item = networkItem.GetComponent<ItemMapObject>();
            item.Setup(itemMapModel, emitterPosition, targetPosition);
        }

        public async UniTask RemoveMapItem(ItemMapObject objectBase)
        {
            var index = _mapProfile.MapItemsList.FindIndex(t => t.ItemEntity.Equals(objectBase.ItemEntity) && t.Position == objectBase.transform.position);

            if (index == -1)
            {
                Debug.LogError($"{nameof(RemoveMapItem)} failed. Item with ID {objectBase.ItemEntity.Id} in position {objectBase.transform.position} not found.");
                return;
            }

            _mapProfile.MapItemsList.RemoveAt(index);
            _mapProfile.Save();

            objectBase.Collider.enabled = false;
            objectBase.transform.parent = _playerService.PlayerController.transform;
            objectBase.SpriteRenderer.sortingLayerID = _playerService.PlayerSortingLayerID.Value;
            await objectBase.transform.DOLocalMove(Vector3.zero, 0.5f).ToUniTask();
            UnityEngine.Object.DestroyImmediate(objectBase.gameObject);
        }

        public (UniTask awaiter, IDisposable disposable) ShowProgressBar(DeviceMapObject deviceMapObject, int rate)
        {
            var progressBar = UnityEngine.Object.Instantiate(_mapConfig.MapObjectProgressBarPrefab, deviceMapObject.ProgressBarContainer);
            return (progressBar.ShowProgressBar(rate), progressBar);
        }
    }
}