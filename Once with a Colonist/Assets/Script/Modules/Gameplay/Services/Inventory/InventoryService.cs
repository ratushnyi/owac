using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Map;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Profiles.Inventory;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Services;
using ItemEntity = TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items.ItemEntity;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory
{
    [UsedImplicitly]
    public class InventoryService : ServiceBase
    {
        private readonly PanelLoader<HUDPanel> _hudPanel;
        private readonly MapService _mapService;
        private readonly InventoryConfig _inventoryConfig;
        private readonly InventoryProfile _inventoryProfile;

        public Func<Vector3Int> GetTargetPosition;

        private InventoryService(
            InventoryProfile inventoryProfile,
            InventoryConfig inventoryConfig,
            MapService mapService,
            PanelLoader<HUDPanel> hudPanel)
        {
            _inventoryProfile = inventoryProfile;
            _inventoryConfig = inventoryConfig;
            _mapService = mapService;
            _hudPanel = hudPanel;
        }

        protected override void Initialize()
        {
            base.Initialize();

            SubscribeOnItemsChanged();
            SubscribeOnItemDropped();
        }

        private void SubscribeOnItemDropped()
        {
            _hudPanel.Instance.SelectedItem.OnButtonClicked.Subscribe(t => Drop(t).Forget()).AddTo(CompositeDisposable);
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

        public bool TryPut(ItemMapObject item)
        {
            return item.Collider.enabled && TryPut(item.ItemEntity.Id, item.ItemEntity.Count, () => _mapService.UnregisterMapItem(item));
        }

        public async UniTask Drop(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }

            _inventoryProfile.InventoryItems[itemId].Value--;
            await _mapService.RegisterMapItem(new ItemEntity { Id = itemId, Count = 1 });
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
                var itemModel = _inventoryConfig[item];
                result = itemModel.Perform(targetPosition);

                if (itemModel.IsCountable && result)
                {
                    _inventoryProfile.InventoryItems[item].Value--;
                }
            }

            return result;
        }
    }
}