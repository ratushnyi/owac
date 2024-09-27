using System.Linq;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Configs.Inventory;
using UniRx;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Profiles.Inventory;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory
{
    public class InventoryPanel : PanelBase
    {
        [SerializeField]
        private Transform _gridContainer;
        private InventoryProfile _inventoryProfile;
        private InventoryConfig _inventoryConfig;
        private InventoryCellView[] _cellsList;
        
        public InventoryCellView FirstCellView => _cellsList?[0];

        [Inject]
        private void Construct(
            InventoryProfile inventoryProfile,
            InventoryConfig inventoryConfig)
        {
            _inventoryProfile = inventoryProfile;
            _inventoryConfig = inventoryConfig;

            _inventoryProfile.InventoryItems.ObserveAdd().Subscribe(Put).AddTo(CompositeDisposable);
        }

        protected override void Initialize()
        {
            _cellsList = new InventoryCellView[_inventoryConfig.InventoryCapacity];
            for (var i = 0; i < _inventoryConfig.InventoryCapacity; i++)
            {
                _cellsList[i] = Instantiate(_inventoryConfig.InventoryCellView, _gridContainer);
                _cellsList[i].OnButtonClicked.Subscribe(onCellClicked).AddTo(CompositeDisposable);

                if (_inventoryProfile.InventoryItems.Count > i)
                {
                    var item = _inventoryProfile.InventoryItems.ElementAt(i);
                    SetItem(_cellsList[i], item.Key, item.Value);
                }
            }

            void onCellClicked(string id)
            {
                _inventoryProfile.SelectedItem.Value = id;
                _inventoryProfile.Save();
            }
        }

        private void Put(DictionaryAddEvent<string, ReactiveProperty<int>> item)
        {
            var cell = _cellsList.FirstOrDefault(t => t.IsEmpty());
            if (cell != null)
            {
                SetItem(cell, item.Key, item.Value);
            }
        }

        private void SetItem(InventoryCellView cell, string id, ReactiveProperty<int> value)
        {
            cell.SetItem(_inventoryConfig[id], value);
        }
    }
}