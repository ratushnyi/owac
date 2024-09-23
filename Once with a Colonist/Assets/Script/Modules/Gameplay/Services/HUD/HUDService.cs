using JetBrains.Annotations;
using UniRx;
using UnityEngine.SceneManagement;
using TendedTarsier.Script.Modules.Gameplay.ToolBar;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.General;
using UnityEngine.EventSystems;

namespace TendedTarsier.Script.Modules.Gameplay.Services.HUD
{
    [UsedImplicitly]
    public class HUDService : ServiceBase
    {
        private readonly EventSystem _eventSystem;
        private readonly InputService _inputService;
        private readonly GeneralConfig _generalConfig;
        private readonly PanelLoader<ToolBarPanel> _toolBarPanel;
        private readonly PanelLoader<InventoryPanel> _inventoryPanel;

        public HUDService(
            EventSystem eventSystem,
            InputService inputService,
            GeneralConfig generalConfig,
            PanelLoader<InventoryPanel> inventoryPanel,
            PanelLoader<ToolBarPanel> toolBarPanel)
        {
            _inventoryPanel = inventoryPanel;
            _toolBarPanel = toolBarPanel;
            _generalConfig = generalConfig;
            _inputService = inputService;
            _eventSystem = eventSystem;
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
                _eventSystem.SetSelectedGameObject(_toolBarPanel.Instance.SelectedItem.gameObject);
            }
            else
            {
                await _inventoryPanel.Show();
                _eventSystem.SetSelectedGameObject(_inventoryPanel.Instance.FirstCellView.gameObject);
            }
        }

        private async void InitHUD()
        {
            var toolBarPanel = await _toolBarPanel.Show();

            toolBarPanel.MenuButton
                .OnClickAsObservable()
                .Subscribe(OnMenuButtonClick)
                .AddTo(CompositeDisposable);

            _eventSystem.SetSelectedGameObject(toolBarPanel.SelectedItem.gameObject);
        }

        private void OnMenuButtonClick(Unit _)
        {
            SceneManager.LoadScene(_generalConfig.MenuScene);
        }
    }
}