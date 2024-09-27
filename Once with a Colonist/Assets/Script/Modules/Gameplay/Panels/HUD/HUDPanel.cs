using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TendedTarsier.Script.Modules.General.Panels;
using TendedTarsier.Script.Modules.General.Profiles.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Configs.Stats;

namespace TendedTarsier.Script.Modules.Gameplay.Panels.HUD
{
    public class HUDPanel : PanelBase
    {
        [field: SerializeField]
        public Button MenuButton { get; set; }

        [field: SerializeField]
        public InventoryCellView SelectedItem { get; set; }

        [field: SerializeField]
        public Transform StatsBarContainer { get; set; }

        private InventoryProfile _inventoryProfile;
        private InventoryConfig _inventoryConfig;
        private StatsConfig _statsConfig;

        private readonly Dictionary<StatType, StatBarView> _statBarViews = new();

        [Inject]
        private void Construct(
            InventoryProfile inventoryProfile,
            InventoryConfig inventoryConfig,
            StatsConfig statsConfig)
        {
            _statsConfig = statsConfig;
            _inventoryConfig = inventoryConfig;
            _inventoryProfile = inventoryProfile;
        }

        protected override void Initialize()
        {
            _inventoryProfile.SelectedItem.Subscribe(itemId =>
            {
                if (string.IsNullOrEmpty(itemId))
                {
                    SelectedItem.SetEmpty();
                    return;
                }
                SelectedItem.SetItem(_inventoryConfig[itemId], _inventoryProfile.InventoryItems[itemId]);
            }).AddTo(CompositeDisposable);
        }

        public StatBarView GetStatBar(StatType statType)
        {
            if (_statBarViews.TryGetValue(statType, out var bar))
            {
                return bar;
            }

            var newStatBar = Instantiate(_statsConfig.StatBarView, StatsBarContainer);
            _statBarViews[statType] = newStatBar;
            return newStatBar;
        }
    }
}