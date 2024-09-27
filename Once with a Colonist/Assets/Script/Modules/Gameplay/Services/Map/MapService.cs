using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General;
using MapItemModel = TendedTarsier.Script.Modules.General.Profiles.Map.MapItemModel;
using MapProfile = TendedTarsier.Script.Modules.General.Profiles.Map.MapProfile;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map
{
    [UsedImplicitly]
    public class MapService : ServiceBase
    {
        public Func<Transform> GetPlayerTransform;
        public Func<int> GetPlayerSortingLayerID;

        private readonly TilemapService _tilemapService;
        private readonly MapProfile _mapProfile;
        private readonly MapConfig _mapConfig;
        private readonly InventoryConfig _inventoryConfig;
        private readonly Transform _propsLayerTransform;

        private MapService(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform propsLayerTransform,
            InventoryConfig inventoryConfig,
            MapConfig mapConfig,
            MapProfile mapProfile,
            TilemapService tilemapService)
        {
            _propsLayerTransform = propsLayerTransform;
            _inventoryConfig = inventoryConfig;
            _mapConfig = mapConfig;
            _mapProfile = mapProfile;
            _tilemapService = tilemapService;
        }

        protected override void Initialize()
        {
            base.Initialize();

            InitMapItems();
        }

        private void InitMapItems()
        {
            foreach (var mapItem in _mapProfile.MapItemsList)
            {
                var item = UnityEngine.Object.Instantiate(_mapConfig.ItemMapObjectPrefab, _propsLayerTransform);
                item.Setup(_inventoryConfig[mapItem.ItemEntity.Id], mapItem, mapItem.Position);
                item.Init(mapItem.LayerID, mapItem.SortingLayerID, _mapConfig.ItemMapActivationDelay);
            }
        }

        public async UniTask DropMapItem(MapItemModel mapItemModel, Vector3 emitterPosition, Vector3 targetPosition)
        {
            var tilemap = _tilemapService.GetTilemap(new Vector2Int(Mathf.CeilToInt(targetPosition.x), Mathf.CeilToInt(targetPosition.y)));
            if (tilemap == null)
            {
                return;
            }

            _mapProfile.MapItemsList.Add(mapItemModel);
            _mapProfile.Save();

            var item = UnityEngine.Object.Instantiate(_mapConfig.ItemMapObjectPrefab, _propsLayerTransform);
            item.Setup(_inventoryConfig[mapItemModel.ItemEntity.Id], mapItemModel, emitterPosition);
            await item.DoMove(targetPosition);
            var sortingLayer = SortingLayer.NameToID(tilemap.GetComponent<Renderer>().sortingLayerName);
            item.Init(tilemap.gameObject.layer, sortingLayer, _mapConfig.ItemMapActivationDelay);
        }

        public async UniTask RemoveMapItem(ItemMapObject objectBase)
        {
            var index = _mapProfile.MapItemsList.FindIndex(t => t.ItemEntity.Equals(objectBase.ItemEntity) && t.Position == objectBase.transform.position);

            if (index == -1)
            {
                Debug.LogError($"{nameof(RemoveMapItem)} failed. Item with ID {objectBase.ItemEntity.Id} in position {objectBase.transform.position} not found.");
                return;
            }

            _mapProfile.MapItemsList.RemoveAt(index);
            _mapProfile.Save();

            objectBase.Collider.enabled = false;
            objectBase.transform.parent = GetPlayerTransform.Invoke();
            objectBase.SpriteRenderer.sortingLayerID = GetPlayerSortingLayerID.Invoke();
            await objectBase.transform.DOLocalMove(Vector3.zero, 0.5f).ToUniTask();
            UnityEngine.Object.DestroyImmediate(objectBase.gameObject);
        }
    }
}