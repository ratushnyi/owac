using JetBrains.Annotations;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;

namespace TendedTarsier.Script.Modules.Gameplay.Inventory
{
    [UsedImplicitly]
    public class HUDService : ServiceBase
    {
        private readonly PanelLoader<InventoryController> _inventoryControllerPanel;
        private readonly InputService _inputService;

        public HUDService(
            InputService inputService,
            PanelLoader<InventoryController> inventoryControllerPanel)
        {
            _inputService = inputService;
            _inventoryControllerPanel = inventoryControllerPanel;
        }
        
        protected override void Initialize()
        {
            SubscribeOnInput();
        }

        private void SubscribeOnInput()
        {
            _inputService.OnYButtonPerformed.Subscribe(_ => SwitchInventory()).AddTo(CompositeDisposable);
        }

        public async void SwitchInventory()
        {
            if (_inventoryControllerPanel.Instance != null)
            {
                await _inventoryControllerPanel.Hide();
            }
            else
            {
                await _inventoryControllerPanel.Show();
            }
        }
    }
}