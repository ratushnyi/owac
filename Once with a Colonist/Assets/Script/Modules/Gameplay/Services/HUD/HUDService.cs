using JetBrains.Annotations;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Profiles.Stats;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Services.Input;
using TendedTarsier.Script.Modules.General.Services.Profile;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using TendedTarsier.Script.Modules.Gameplay.Panels.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Configs.Stats;

namespace TendedTarsier.Script.Modules.Gameplay.Services.HUD
{
    [UsedImplicitly]
    public class HUDService : ServiceBase
    {
        private readonly EventSystem _eventSystem;
        private readonly ProfileService _profileService;
        private readonly InputService _inputService;
        private readonly GeneralConfig _generalConfig;
        private readonly PanelLoader<HUDPanel> _hudPanel;
        private readonly PanelLoader<InventoryPanel> _inventoryPanel;

        public HUDService(
            EventSystem eventSystem,
            ProfileService profileService,
            InputService inputService,
            GeneralConfig generalConfig,
            PanelLoader<InventoryPanel> inventoryPanel,
            PanelLoader<HUDPanel> hudPanel)
        {
            _inventoryPanel = inventoryPanel;
            _hudPanel = hudPanel;
            _generalConfig = generalConfig;
            _inputService = inputService;
            _profileService = profileService;
            _eventSystem = eventSystem;
        }

        protected override void Initialize()
        {
            base.Initialize();

            SubscribeOnInput();
        }

        private void SubscribeOnInput()
        {
            _inputService.OnYButtonPerformed
                .Subscribe(_ => SwitchInventory())
                .AddTo(CompositeDisposable);

            _inputService.OnMenuButtonPerformed
                .Subscribe(_ => OnMenuButtonClick())
                .AddTo(CompositeDisposable);

            _hudPanel.Instance.MenuButton
                .OnClickAsObservable()
                .Subscribe(_ => OnMenuButtonClick())
                .AddTo(CompositeDisposable);

            _eventSystem.SetSelectedGameObject(_hudPanel.Instance.SelectedItem.gameObject);
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

        private void OnMenuButtonClick()
        {
            _profileService.SaveAll();
            SceneManager.LoadScene(_generalConfig.MenuScene);
        }

        public void ShowStatBar(StatType statType, StatModel statModel, StatProfileElement statProfile)
        {
            var statBar = _hudPanel.Instance.GetStatBar(statType);

            statBar.Setup(statProfile.Value.Value, statProfile.Range.Value);
            statBar.SetSprite(statModel.Sprite);
            statProfile.Value.SkipLatestValueOnSubscribe().Subscribe(t => statBar.UpdateValue(t)).AddTo(CompositeDisposable);
            statProfile.Range.SkipLatestValueOnSubscribe().Subscribe(t => statBar.UpdateRange(t)).AddTo(CompositeDisposable);
        }
    }
}