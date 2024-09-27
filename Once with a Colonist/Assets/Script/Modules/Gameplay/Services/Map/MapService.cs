using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map
{
    [UsedImplicitly]
    public class MapService : ServiceBase
    {
        public Func<Vector3Int> GetTargetDirection;
        public Func<Transform> GetPlayerTransform;
        public Func<int> GetPlayerSortingLayerID;

        private readonly StatsService _statsService;
        private readonly TilemapService _tilemapService;
        private readonly MapProfile _mapProfile;
        private readonly MapConfig _mapConfig;
        private readonly InventoryConfig _inventoryConfig;
        private readonly Transform _propsLayerTransform;

        private MapService(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform propsLayerTransform,
            InventoryConfig inventoryConfig,
            MapConfig mapConfig,
            MapProfile mapProfile,
            TilemapService tilemapService,
            StatsService statsService)
        {
            _propsLayerTransform = propsLayerTransform;
            _inventoryConfig = inventoryConfig;
            _mapConfig = mapConfig;
            _mapProfile = mapProfile;
            _tilemapService = tilemapService;
            _statsService = statsService;
        }

        protected override void Initialize()
        {
            base.Initialize();

            InitMapItems();
        }

        private void InitMapItems()
        {
            foreach (var mapItem in _mapProfile.MapItemsList)
            {
                var item = UnityEngine.Object.Instantiate(_mapConfig.ItemMapObjectPrefab, _propsLayerTransform);
                item.ItemEntity = mapItem.ItemEntity;
                item.SpriteRenderer.sprite = _inventoryConfig[mapItem.ItemEntity.Id].Sprite;
                item.SpriteRenderer.sortingLayerID = mapItem.SortingLayerID;
                item.transform.position = mapItem.Position;
                item.gameObject.layer = mapItem.LayerID;
            }
        }

        public async UniTask RegisterMapItem(ItemEntity itemEntity)
        {
            var playerTransform = GetPlayerTransform.Invoke();
            var targetDirection = GetTargetDirection.Invoke();
            var playerSortingLayer = GetPlayerSortingLayerID.Invoke();

            var item = UnityEngine.Object.Instantiate(_mapConfig.ItemMapObjectPrefab, _propsLayerTransform);
            item.ItemEntity.Count = 1;
            item.Collider.enabled = false;
            item.ItemEntity = itemEntity;
            item.SpriteRenderer.sprite = _inventoryConfig[itemEntity.Id].Sprite;
            item.SpriteRenderer.sortingLayerID = playerSortingLayer;
            item.transform.position = playerTransform.position;
            var targetPosition = playerTransform.position + targetDirection * _statsService.DropDistance;
            var tilemap = _tilemapService.GetTilemap(new Vector2Int(Mathf.CeilToInt(targetPosition.x), Mathf.CeilToInt(targetPosition.y)));
            var sortingLayerID = SortingLayer.NameToID(tilemap.GetComponent<Renderer>().sortingLayerName);
            await item.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad).ToUniTask();
            item.SpriteRenderer.sortingLayerID = sortingLayerID;
            item.gameObject.layer = tilemap.gameObject.layer;
            item.Collider.enabled = true;
            _mapProfile.MapItemsList.Add(new MapItemModel
            {
                Position = item.transform.position,
                ItemEntity = itemEntity,
                SortingLayerID = sortingLayerID
            });
            _mapProfile.Save();
        }

        public async UniTask UnregisterMapItem(ItemMapObject objectBase)
        {
            var index = _mapProfile.MapItemsList.FindIndex(t => t.ItemEntity.Equals(objectBase.ItemEntity) && t.Position == objectBase.transform.position);

            if (index == -1)
            {
                Debug.LogError($"{nameof(UnregisterMapItem)} failed. Item with ID {objectBase.ItemEntity.Id} in position {objectBase.transform.position} not found.");
                return;
            }

            _mapProfile.MapItemsList.RemoveAt(index);
            _mapProfile.Save();

            objectBase.Collider.enabled = false;
            objectBase.transform.parent = GetPlayerTransform.Invoke();
            objectBase.SpriteRenderer.sortingLayerID = GetPlayerSortingLayerID.Invoke();
            await objectBase.transform.DOLocalMove(Vector3.zero, 0.5f).ToUniTask();
            UnityEngine.Object.DestroyImmediate(objectBase.gameObject);
        }
    }
}