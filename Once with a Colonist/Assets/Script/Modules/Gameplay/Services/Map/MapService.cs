using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Configs.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using UnityEngine;
using Zenject;
using ItemEntity = TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items.ItemEntity;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map
{
    [UsedImplicitly]
    public class MapService : ServiceBase
    {
        public Func<Vector3Int> GetTargetDirection;
        public Func<Transform> GetPlayerTransform;
        public Func<int> GetPlayerSortingLayerID;

        private readonly StatsService _statsService;
        private readonly MapProfile _mapProfile;
        private readonly TilemapService _tilemapService;
        private readonly InventoryConfig _inventoryConfig;
        private readonly Transform _propsLayerTransform;

        private MapService(
            [Inject(Id = GameplayInstaller.MapItemsContainerTransformId)] Transform propsLayerTransform,
            InventoryConfig inventoryConfig,
            MapProfile mapProfile,
            TilemapService tilemapService,
            StatsService statsService)
        {
            _propsLayerTransform = propsLayerTransform;
            _inventoryConfig = inventoryConfig;
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
                var item = UnityEngine.Object.Instantiate(_inventoryConfig.MapItemPrefab, _propsLayerTransform);
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

            var item = UnityEngine.Object.Instantiate(_inventoryConfig.MapItemPrefab, _propsLayerTransform);
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
            item.Collider.enabled = false;
            _mapProfile.MapItemsList.Add(new MapItemModel
            {
                Position = item.transform.position,
                ItemEntity = itemEntity,
                SortingLayerID = sortingLayerID
            });
            _mapProfile.Save();
        }

        public async UniTask UnregisterMapItem(MapItem.MapItem item)
        {
            var index = _mapProfile.MapItemsList.FindIndex(t => t.ItemEntity.Equals(item.ItemEntity) && t.Position == item.transform.position);

            if (index == -1)
            {
                Debug.LogError($"{nameof(UnregisterMapItem)} failed. Item with ID {item.ItemEntity.Id} in position {item.transform.position} not found.");
                return;
            }

            _mapProfile.MapItemsList.RemoveAt(index);
            _mapProfile.Save();

            item.Collider.enabled = false;
            item.transform.parent = GetPlayerTransform.Invoke();
            item.SpriteRenderer.sortingLayerID = GetPlayerSortingLayerID.Invoke();
            await item.transform.DOLocalMove(Vector3.zero, 0.5f).ToUniTask();
            UnityEngine.Object.DestroyImmediate(item.gameObject);
        }
    }
}