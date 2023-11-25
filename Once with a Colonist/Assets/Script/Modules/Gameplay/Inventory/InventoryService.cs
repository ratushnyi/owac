using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace TendedTarsier
{
    public class InventoryService
    {
        private readonly InventoryProfile _inventoryProfile;
        private readonly InventoryConfig _inventoryConfig;
        private readonly PlayerController _playerController;
        private readonly Canvas _gameplayCanvas;
        
        private InventoryPanel _inventoryPanel;

        private InventoryService(InventoryProfile inventoryProfile, InventoryConfig inventoryConfig, PlayerController playerController, Canvas gameplayCanvas)
        {
            _gameplayCanvas = gameplayCanvas;
            _playerController = playerController;
            _inventoryConfig = inventoryConfig;
            _inventoryProfile = inventoryProfile;
        }
        
        public void SwitchInventory()
        {
            if (_inventoryPanel != null)
            {
                HideInventory();
            }
            else
            {
                ShowInventory();
            }
        }

        public void ShowInventory()
        {
            if (_inventoryPanel != null)
            {
                return;
            }

            _inventoryPanel = Object.Instantiate(_inventoryConfig.InventoryPanel, _gameplayCanvas.transform);
            _inventoryPanel.Init(_inventoryProfile, _inventoryConfig);
        }

        public void HideInventory()
        {
            if (_inventoryPanel != null)
            {
                Object.DestroyImmediate(_inventoryPanel.gameObject);
            }
        }

        public bool TryPut(MapItemBase item)
        {
            if (!item.Collider.enabled)
            {
                return false;
            }
            
            var existItem = _inventoryProfile.InventoryItems.FirstOrDefault(t => t.Key == item.Id);
            if (existItem.Key != null)
            {
                addExistItem();
                return true;
            }
            
            if (_inventoryProfile.InventoryItems.Count >= 9)
            {
                return false;
            }

            addNewItem();
            return true;

            async void addExistItem()
            {
                await putObject();
                existItem.Value.Value += item.Count;
                _inventoryProfile.Save();
            }
            
            async void addNewItem()
            {
                await putObject();
                var count = new ReactiveProperty<int>(item.Count);
                _inventoryProfile.InventoryItems.Add(item.Id, count);
                _inventoryProfile.Save();
            }

            async UniTask putObject()
            {
                item.Collider.enabled = false;
                item.transform.parent = _playerController.transform;
                await item.transform.DOLocalMove(Vector3.zero, 0.5f).ToUniTask();
                Object.DestroyImmediate(item.gameObject);
            }
        }
    }
}
