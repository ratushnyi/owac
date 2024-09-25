using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Configs.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Field;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps
{
    [UsedImplicitly]
    public class MapService : ServiceBase
    {
        public Func<Vector3Int> GetTargetDirection;
        public Func<Vector3> GetCharacterPosition;

        private readonly StatsService _statsService;
        private readonly MapProfile _mapProfile;
        private readonly InventoryConfig _inventoryConfig;
        private readonly Transform _propsLayerTransform;

        private MapService(
            [Inject(Id = GameplayInstaller.MapItemsContainerTransformId)] 
            Transform propsLayerTransform,
            InventoryConfig inventoryConfig,
            MapProfile mapProfile,
            StatsService statsService)
        {
            _propsLayerTransform = propsLayerTransform;
            _inventoryConfig = inventoryConfig;
            _mapProfile = mapProfile;
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
                item.transform.position = mapItem.Position;
            }
        }

        public async UniTask RegisterMapItem(ItemEntity itemEntity)
        {
            var characterPosition = GetCharacterPosition.Invoke();
            var targetDirection = GetTargetDirection.Invoke();
            var item = UnityEngine.Object.Instantiate(_inventoryConfig.MapItemPrefab, _propsLayerTransform);
            item.ItemEntity.Count = 1;
            item.Collider.enabled = false;
            item.ItemEntity = itemEntity;
            item.SpriteRenderer.sprite = _inventoryConfig[itemEntity.Id].Sprite;
            item.transform.position = characterPosition;
            await item.transform.DOMove(characterPosition + targetDirection * _statsService.DropDistance, 0.5f).SetEase(Ease.OutQuad).ToUniTask();
            item.Collider.enabled = true;

            _mapProfile.MapItemsList.Add(new MapItemModel { Position = item.transform.position, ItemEntity = itemEntity });
            _mapProfile.Save();
        }

        public void UnregisterMapItem(MapItem item)
        {
            var index = _mapProfile.MapItemsList.FindIndex(t => t.ItemEntity.Equals(item.ItemEntity) && t.Position == item.transform.position);

            if (index == -1)
            {
                Debug.LogError($"{nameof(UnregisterMapItem)} failed. Item with ID {item.ItemEntity.Id} in position {item.transform.position} not found.");
                return;
            }

            _mapProfile.MapItemsList.RemoveAt(index);
            _mapProfile.Save();
        }
    }
}