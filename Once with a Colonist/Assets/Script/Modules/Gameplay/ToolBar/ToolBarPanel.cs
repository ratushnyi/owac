using Cysharp.Threading.Tasks;
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

        private InventoryService _inventoryService;
        private InventoryProfile _inventoryProfile;
        private InventoryConfig _inventoryConfig;

        [Inject]
        private void Construct(InventoryService inventoryService, InventoryProfile inventoryProfile, InventoryConfig inventoryConfig)
        {
            _inventoryConfig = inventoryConfig;
            _inventoryProfile = inventoryProfile;
            _inventoryService = inventoryService;
        }

        protected override void Initialize()
        {
            _inventoryProfile.SelectedItem.Subscribe(itemId =>
            {
                if (string.IsNullOrEmpty(itemId))
                {
                    return;
                }
                SelectedItem.SetItem(_inventoryConfig[itemId], _inventoryProfile.InventoryItems[itemId]);
            }).AddTo(CompositeDisposable);

            SelectedItem.OnButtonClicked.Subscribe(t => _inventoryService.Drop(t).Forget());
        }
    }
}