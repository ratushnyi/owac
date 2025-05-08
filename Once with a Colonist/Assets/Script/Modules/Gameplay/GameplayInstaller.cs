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
using Unity.Netcode;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public class GameplayInstaller : MonoInstaller, IInitializable
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
        [SerializeField]
        private NetworkObject _networkServiceHandler;

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
        }

        public void Initialize()
        {
            BindInventoryItems();
        }

        private void BindInventoryItems()
        {
            TypeAnalyzer.ShouldAllowDuringValidation<InventoryConfig>();
            Container.Resolve<InventoryConfig>().InventoryItems.ForEach(t => Container.Inject(t.Tool));
        }

        private void BindServices()
        {
            Container.BindService<TilemapService>(_networkServiceHandler);
            Container.BindService<InventoryService>(_networkServiceHandler);
            Container.BindService<HUDService>(_networkServiceHandler);
            Container.BindService<StatsService>(_networkServiceHandler);
            Container.BindService<MapService>(_networkServiceHandler);
            Container.BindService<PlayerService>(_networkServiceHandler);
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