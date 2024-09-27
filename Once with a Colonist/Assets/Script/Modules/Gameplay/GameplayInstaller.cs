using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using TendedTarsier.Script.Modules.Gameplay.Panels.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Map;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Common")]
        [SerializeField]
        private PlayerController _playerController;
        [SerializeField]
        private PlayerProgressBarController _playerProgressBarController;
        [SerializeField]
        private Transform _mapItemsContainerTransform;
        [SerializeField]
        private List<Tilemap> _groundTilemapsList;

        [Header("UI")]
        [SerializeField]
        private Camera _gameplayCamera;
        [SerializeField]
        private Canvas _gameplayCanvas;

        [Header("Panels")]
        [SerializeField]
        private InputPanel _inputPanel;
        [SerializeField]
        private HUDPanel _hudPanel;
        [SerializeField]
        private InventoryPanel _inventoryPanel;

        public override void InstallBindings()
        {
            BindSystem();
            BindServices();
            BindPanels();
            BindSceneObjects();
            BindInventoryItems();
        }

        private void BindInventoryItems()
        {
            Container.Resolve<InventoryConfig>().InventoryItems.ForEach(t => Container.Inject(t.Tool));
        }

        private void BindServices()
        {
            Container.BindService<TilemapService>();
            Container.BindService<InventoryService>();
            Container.BindService<HUDService>();
            Container.BindService<StatsService>();
            Container.BindService<MapService>();
        }

        private void BindSystem()
        {
            Container.Bind<Canvas>().FromInstance(_gameplayCanvas).NonLazy();
            Container.Bind<Camera>().FromInstance(_gameplayCamera).NonLazy();
        }

        private void BindPanels()
        {
            Container.BindPanel<InputPanel>(_inputPanel, _gameplayCanvas);
            Container.BindPanel<HUDPanel>(_hudPanel, _gameplayCanvas);
            Container.BindPanel<InventoryPanel>(_inventoryPanel, _gameplayCanvas);
        }

        private void BindSceneObjects()
        {
            Container.Bind<PlayerProgressBarController>().FromInstance(_playerProgressBarController).AsSingle();
            Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
            Container.Bind<List<Tilemap>>().WithId(GeneralConstants.GroundTilemapsListId).FromInstance(_groundTilemapsList);
            Container.Bind<Transform>().WithId(GeneralConstants.MapItemsContainerTransformId).FromInstance(_mapItemsContainerTransform);
        }
    }
}