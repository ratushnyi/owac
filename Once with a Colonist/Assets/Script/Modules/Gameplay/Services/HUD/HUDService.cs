using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using UniRx;
using UnityEngine.SceneManagement;
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
        private readonly PanelLoader<HUDPanel> _hudPanel;
        private readonly PanelLoader<InventoryPanel> _inventoryPanel;

        public HUDService(
            EventSystem eventSystem,
            InputService inputService,
            GeneralConfig generalConfig,
            PanelLoader<InventoryPanel> inventoryPanel,
            PanelLoader<HUDPanel> hudPanel)
        {
            _inventoryPanel = inventoryPanel;
            _hudPanel = hudPanel;
            _generalConfig = generalConfig;
            _inputService = inputService;
            _eventSystem = eventSystem;
        }

        protected override void Initialize()
        {
            base.Initialize();

            SubscribeOnInput();
            InitHUD();
        }

        private void SubscribeOnInput()
        {
            _inputService.OnYButtonPerformed
                .Subscribe(_ => SwitchInventory())
                .AddTo(CompositeDisposable);
        }

        private async void SwitchInventory()
        {
            if (_inventoryPanel.Instance != null)
            {
                await _inventoryPanel.Hide();
                _eventSystem.SetSelectedGameObject(_hudPanel.Instance.SelectedItem.gameObject);
            }
            else
            {
                await _inventoryPanel.Show();
                _eventSystem.SetSelectedGameObject(_inventoryPanel.Instance.FirstCellView.gameObject);
            }
        }

        private void InitHUD()
        {
            _hudPanel.Instance.MenuButton
                .OnClickAsObservable()
                .Subscribe(OnMenuButtonClick)
                .AddTo(CompositeDisposable);
            
            _eventSystem.SetSelectedGameObject(_hudPanel.Instance.SelectedItem.gameObject);
        }

        private void OnMenuButtonClick(Unit _)
        {
            SceneManager.LoadScene(_generalConfig.MenuScene);
        }

        public void ShowStatBar(StatType statType, ReactiveProperty<int> value, ReactiveProperty<int> range)
        {
            var statBar = _hudPanel.Instance.GetStatBar(statType);

            statBar.Setup(value.Value, range.Value);
            value.SkipLatestValueOnSubscribe().Subscribe(t => statBar.UpdateValue(t)).AddTo(CompositeDisposable);
            range.SkipLatestValueOnSubscribe().Subscribe(t => statBar.UpdateRange(t)).AddTo(CompositeDisposable);
        }
    }
}