using JetBrains.Annotations;
using UniRx;
using UnityEngine.SceneManagement;
using TendedTarsier.Script.Modules.Gameplay.ToolBar;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;

namespace TendedTarsier.Script.Modules.Gameplay.Services.HUD
{
    [UsedImplicitly]
    public class HUDService : ServiceBase
    {
        private readonly InputService _inputService;
        private readonly GeneralConfig _generalConfig;
        private readonly PanelLoader<ToolBarPanel> _toolBarPanel;
        private readonly PanelLoader<InventoryPanel> _inventoryPanel;

        public HUDService(
            InputService inputService,
            GeneralConfig generalConfig,
            PanelLoader<InventoryPanel> inventoryPanel,
            PanelLoader<ToolBarPanel> toolBarPanel)
        {
            _inventoryPanel = inventoryPanel;
            _toolBarPanel = toolBarPanel;
            _generalConfig = generalConfig;
            _inputService = inputService;
        }

        protected override void Initialize()
        {
            SubscribeOnInput();
            InitHUD();
        }

        private void SubscribeOnInput()
        {
            _inputService.OnYButtonPerformed
                .Subscribe(_ => SwitchInventory())
                .AddTo(CompositeDisposable);
        }

        public async void SwitchInventory()
        {
            if (_inventoryPanel.Instance != null)
            {
                await _inventoryPanel.Hide();
            }
            else
            {
                await _inventoryPanel.Show();
            }
        }

        private async void InitHUD()
        {
            await _toolBarPanel.Show();

            _toolBarPanel.Instance.MenuButton
                .OnClickAsObservable()
                .Subscribe(OnMenuButtonClick)
                .AddTo(CompositeDisposable);
        }

        private void OnMenuButtonClick(Unit _)
        {
            SceneManager.LoadScene(_generalConfig.MenuScene);
        }
    }
}