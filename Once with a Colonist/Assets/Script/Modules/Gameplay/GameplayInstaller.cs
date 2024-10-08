using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using TendedTarsier.Script.Modules.Gameplay.Panels.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.HUD;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory;
using TendedTarsier.Script.Modules.Gameplay.Services.Map;
using TendedTarsier.Script.Modules.Gameplay.Services.Player;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("SceneObjects")]
        [SerializeField]
        private Transform _mapItemsContainerTransform;
        [SerializeField]
        private List<Tilemap> _groundTilemapsList;
        
        [Header("System")]
        [SerializeField]
        private Canvas _gameplayCanvas;
        [SerializeField]
        private Camera _gameplayCamera;
        [SerializeField]
        private CinemachineVirtualCamera _cinemachineCamera;

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
            TypeAnalyzer.ShouldAllowDuringValidation<InventoryConfig>();
            Container.Resolve<InventoryConfig>().InventoryItems.ForEach(t => Container.Inject(t.Tool));
        }

        private void BindServices()
        {
            Container.BindService<TilemapService>();
            Container.BindService<InventoryService>();
            Container.BindService<HUDService>();
            Container.BindService<StatsService>();
            Container.BindService<MapService>();
            Container.BindService<PlayerService>();
        }

        private void BindSystem()
        {
            Container.Bind<Canvas>().FromInstance(_gameplayCanvas).NonLazy();
            Container.Bind<Camera>().FromInstance(_gameplayCamera).NonLazy();
            Container.Bind<CinemachineVirtualCamera>().FromInstance(_cinemachineCamera).NonLazy();
        }

        private void BindPanels()
        {
            Container.BindPanel<InputPanel>(_inputPanel, _gameplayCanvas);
            Container.BindPanel<HUDPanel>(_hudPanel, _gameplayCanvas);
            Container.BindPanel<InventoryPanel>(_inventoryPanel, _gameplayCanvas);
        }

        private void BindSceneObjects()
        {
            Container.Bind<List<Tilemap>>().WithId(GeneralConstants.GroundTilemapsListId).FromInstance(_groundTilemapsList);
            Container.Bind<Transform>().WithId(GeneralConstants.MapItemsContainerTransformId).FromInstance(_mapItemsContainerTransform);
        }
    }
}