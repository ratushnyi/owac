using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;

namespace TendedTarsier.Script.Modules.Gameplay.ToolBar
{
    public class ToolBarPanel : PanelBase
    {
        [field: SerializeField]
        public Button MenuButton { get; set; }
        [field: SerializeField]
        public InventoryCellView SelectedItem { get; set; }

        private InventoryProfile _inventoryProfile;
        private InventoryConfig _inventoryConfig;

        [Inject]
        private void Construct(InventoryProfile inventoryProfile, InventoryConfig inventoryConfig)
        {
            _inventoryConfig = inventoryConfig;
            _inventoryProfile = inventoryProfile;
        }

        private void Start()
        {
            _inventoryProfile.SelectedItem.Subscribe(itemId =>
            {
                if (string.IsNullOrEmpty(itemId))
                {
                    return;
                }
                SelectedItem.SetItem(_inventoryConfig[itemId], _inventoryProfile.InventoryItems[itemId]);
            }).AddTo(CompositeDisposable);
        }
    }
}