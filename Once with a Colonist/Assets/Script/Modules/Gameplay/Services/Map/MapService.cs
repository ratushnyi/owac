using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.Gameplay.Services.Player;
using TendedTarsier.Script.Modules.General.Profiles.Map;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map
{
    [UsedImplicitly]
    public class MapService : ServiceBase
    {
        private readonly TilemapService _tilemapService;
        private readonly PlayerService _playerService;
        private readonly MapProfile _mapProfile;
        private readonly MapConfig _mapConfig;
        private readonly InventoryConfig _inventoryConfig;
        private readonly Transform _mapItemsContainer;

        private MapService(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform mapItemsContainer,
            InventoryConfig inventoryConfig,
            MapConfig mapConfig,
            MapProfile mapProfile,
            PlayerService playerService,
            TilemapService tilemapService)
        {
            _mapItemsContainer = mapItemsContainer;
            _inventoryConfig = inventoryConfig;
            _mapConfig = mapConfig;
            _mapProfile = mapProfile;
            _playerService = playerService;
            _tilemapService = tilemapService;
        }

        public override void Initialize()
        {
            InitMapItems();
        }

        private void InitMapItems()
        {
            foreach (var mapItem in _mapProfile.MapItemsList)
            {
                var item = UnityEngine.Object.Instantiate(_mapConfig.ItemMapObjectPrefab, _mapItemsContainer);
                item.Setup(_inventoryConfig[mapItem.ItemEntity.Id], mapItem, mapItem.Position);
                item.Init(mapItem.LayerID, mapItem.SortingLayerID, _mapConfig.ItemMapActivationDelay);
            }
        }

        public async UniTask DropMapItem(ItemMapModel itemMapModel, Vector3 emitterPosition, Vector3 targetPosition)
        {
            var tilemap = _tilemapService.GetTilemap(new Vector2Int(Mathf.CeilToInt(targetPosition.x), Mathf.CeilToInt(targetPosition.y)));
            if (tilemap == null)
            {
                return;
            }

            _mapProfile.MapItemsList.Add(itemMapModel);
            _mapProfile.Save();

            var item = UnityEngine.Object.Instantiate(_mapConfig.ItemMapObjectPrefab, _mapItemsContainer);
            item.Setup(_inventoryConfig[itemMapModel.ItemEntity.Id], itemMapModel, emitterPosition);
            await item.DoMove(targetPosition);
            var sortingLayer = SortingLayer.NameToID(tilemap.GetComponent<Renderer>().sortingLayerName);
            item.Init(tilemap.gameObject.layer, sortingLayer, _mapConfig.ItemMapActivationDelay);
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