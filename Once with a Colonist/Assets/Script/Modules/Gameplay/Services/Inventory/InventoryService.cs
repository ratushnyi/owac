using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Configs.Inventory;
using UniRx;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Field;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General.Services;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory
{
    [UsedImplicitly]
    public class InventoryService : ServiceBase
    {
        private readonly StatsService _statsService;
        private readonly TilemapService _tilemapService;
        private readonly InventoryConfig _inventoryConfig;
        private readonly General.Profiles.InventoryProfile _inventoryProfile;
        private readonly Transform _propsLayerTransform;

        public Func<Vector3Int> GetTargetDirection;
        public Func<Vector3Int> GetTargetPosition;
        public Func<Vector3> GetCharacterPosition;

        private InventoryService(
            [Inject(Id = GameplayInstaller.PropsTransformId)] Transform propsLayerTransform,
            General.Profiles.InventoryProfile inventoryProfile,
            InventoryConfig inventoryConfig,
            TilemapService tilemapService,
            StatsService statsService)
        {
            _propsLayerTransform = propsLayerTransform;
            _inventoryProfile = inventoryProfile;
            _inventoryConfig = inventoryConfig;
            _tilemapService = tilemapService;
            _statsService = statsService;
        }

        protected override void Initialize()
        {
            SubscribeOnItemsChanged();
        }

        private void SubscribeOnItemsChanged()
        {
            foreach (var item in _inventoryProfile.InventoryItems)
            {
                subscribe(item.Key, item.Value);
            }

            _inventoryProfile.InventoryItems.ObserveAdd().Subscribe(t => subscribe(t.Key, t.Value)).AddTo(CompositeDisposable);

            void subscribe(string id, ReactiveProperty<int> value)
            {
                var disposable = value.SkipLatestValueOnSubscribe().Subscribe(count => onCountChanged(count, id))
                    .AddTo(CompositeDisposable);

                _inventoryProfile.InventoryItems.ObserveRemove()
                    .Where(t => t.Key == id)
                    .First()
                    .Subscribe(_ => onItemRemoved(disposable));
            }

            void onCountChanged(int count, string id)
            {
                if (count == 0)
                {
                    _inventoryProfile.InventoryItems.Remove(id);
                    if (_inventoryProfile.SelectedItem.Value == id)
                    {
                        _inventoryProfile.SelectedItem.Value = null;
                    }
                }

                _inventoryProfile.Save();
            }

            void onItemRemoved(IDisposable disposable)
            {
                CompositeDisposable.Remove(disposable);
            }
        }

        public bool TryPut(string id, int count, Func<UniTask> beforeItemAdd = null)
        {
            var existItem = _inventoryProfile.InventoryItems.FirstOrDefault(t => t.Key == id);
            if (existItem.Key != null)
            {
                addExistItem();
                return true;
            }

            if (_inventoryProfile.InventoryItems.Count >= _inventoryConfig.InventoryCapacity)
            {
                return false;
            }

            addNewItem();
            return true;

            async void addExistItem()
            {
                if (beforeItemAdd != null)
                {
                    await beforeItemAdd.Invoke();
                }
                existItem.Value.Value += count;
            }

            async void addNewItem()
            {
                if (beforeItemAdd != null)
                {
                    await beforeItemAdd.Invoke();
                }
                existItem = _inventoryProfile.InventoryItems.FirstOrDefault(t => t.Key == id);
                if (existItem.Key != null)
                {
                    existItem.Value.Value += count;
                }
                else
                {
                    var property = new ReactiveProperty<int>(count);
                    _inventoryProfile.InventoryItems.Add(id, property);
                }
            }
        }

        public bool TryPut(MapItemBase item, Transform parent)
        {
            return item.Collider.enabled && TryPut(item.Id, item.Count, putObject);

            async UniTask putObject()
            {
                item.Collider.enabled = false;
                item.transform.parent = parent;
                await item.transform.DOLocalMove(Vector3.zero, 0.5f).ToUniTask();
                UnityEngine.Object.DestroyImmediate(item.gameObject);
            }
        }

        public async UniTask Drop(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || GetCharacterPosition == null || GetTargetDirection == null)
            {
                return;
            }

            var characterPosition = GetCharacterPosition.Invoke();
            var targetDirection = GetTargetDirection.Invoke();
            var item = UnityEngine.Object.Instantiate(_inventoryConfig.MapItemPrefab, _propsLayerTransform);
            item.Count = 1;
            item.Collider.enabled = false;
            item.Id = itemId;
            item.SpriteRenderer.sprite = _inventoryConfig[item.Id].Sprite;
            _inventoryProfile.InventoryItems[item.Id].Value--;
            item.transform.position = characterPosition;

            await item.transform.DOMove(characterPosition + targetDirection * _statsService.DropDistance, 0.5f).SetEase(Ease.OutQuad).ToUniTask();
            item.Collider.enabled = true;
        }

        public bool Perform()
        {
            if (GetTargetPosition == null)
            {
                return false;
            }

            var result = false;
            var targetPosition = GetTargetPosition.Invoke();
            var item = _inventoryProfile.SelectedItem.Value;
            if (!string.IsNullOrEmpty(item))
            {
                result = _inventoryConfig[item].Perform(_tilemapService.CurrentTilemap.Value, targetPosition);

                if (result)
                {
                    _inventoryProfile.InventoryItems[item].Value--;
                }
            }

            return result;
        }
    }
}