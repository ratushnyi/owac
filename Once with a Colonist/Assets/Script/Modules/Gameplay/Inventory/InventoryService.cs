using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace TendedTarsier
{
    [UsedImplicitly]
    public class InventoryService
    {
        private readonly InventoryProfile _inventoryProfile;
        private readonly PlayerController _playerController;
        private readonly PanelLoader<InventoryController> _inventoryControllerPanel;

        private InventoryService(InventoryProfile inventoryProfile, PlayerController playerController, PanelLoader<InventoryController> inventoryControllerPanel)
        {
            _inventoryControllerPanel = inventoryControllerPanel;
            _playerController = playerController;
            _inventoryProfile = inventoryProfile;
        }

        public void SwitchInventory()
        {
            if (_inventoryControllerPanel.Instance != null)
            {
                _inventoryControllerPanel.Hide();
            }
            else
            {
                _inventoryControllerPanel.Show();
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