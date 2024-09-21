using TendedTarsier.Script.Modules.Gameplay.Configs;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TendedTarsier
{
    public class ToolBarController : MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new();

        [field: SerializeField]
        public Button MenuButton { get; set; }
        [field: SerializeField]
        public InventoryCellView SelectedItem { get; set; }

        private Script.Modules.Gameplay.Services.Inventory.InventoryProfile _inventoryProfile;
        private InventoryConfig _inventoryConfig;

        [Inject]
        private void Construct(Script.Modules.Gameplay.Services.Inventory.InventoryProfile inventoryProfile, InventoryConfig inventoryConfig)
        {
            _inventoryConfig = inventoryConfig;
            _inventoryProfile = inventoryProfile;
        }

        private void Start()
        {
            _inventoryProfile.SelectedItem.Subscribe(t =>
            {
                if (string.IsNullOrEmpty(t))
                {
                    return;
                }
                SelectedItem.SetItem(_inventoryConfig[t], _inventoryProfile.InventoryItems[t]);
            }).AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }
    }
}