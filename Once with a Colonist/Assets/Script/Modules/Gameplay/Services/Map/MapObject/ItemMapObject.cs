using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using TendedTarsier.Script.Modules.General.Configs;
using TendedTarsier.Script.Modules.General.Profiles.Map;
using Unity.Netcode;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class ItemMapObject : MapObjectBase
    {
        private readonly NetworkVariable<ItemMapModel> _itemMapModel = new();
        private readonly NetworkVariable<Vector3> _basePosition = new();
        private readonly NetworkVariable<Vector3> _targetPosition = new();
        private readonly NetworkVariable<bool> _targetPositionExist = new();

        private Transform _mapItemsContainer;
        private MapConfig _mapConfig;
        private InventoryConfig _inventoryConfig;

        [field: SerializeField]
        public ItemEntity ItemEntity { get; set; }

        [Inject]
        private void Construct(
            [Inject(Id = GeneralConstants.MapItemsContainerTransformId)] Transform mapItemsContainer,
            MapConfig mapConfig,
            InventoryConfig inventoryConfig)
        {
            _mapItemsContainer = mapItemsContainer;
            _mapConfig = mapConfig;
            _inventoryConfig = inventoryConfig;
        }

        protected override void OnNetworkInitialized()
        {
            Initialize().Forget();
        }

        public void Setup(ItemMapModel itemMapModel, Vector3 basePosition, Vector3? targetPosition = null)
        {
            _itemMapModel.Value = itemMapModel;
            _basePosition.Value = basePosition;
            if (targetPosition.HasValue)
            {
                _targetPositionExist.Value = true;
                _targetPosition.Value = targetPosition.Value;
            }
        }
        
        private async UniTaskVoid Initialize()
        {
            Collider.enabled = false;
            ItemEntity = _itemMapModel.Value.ItemEntity;
            NetworkObject.TrySetParent(_mapItemsContainer);
            SpriteRenderer.sprite = _inventoryConfig[_itemMapModel.Value.ItemEntity.Id].Sprite;
            SpriteRenderer.sortingLayerID = _itemMapModel.Value.SortingLayerID;
            gameObject.layer = _itemMapModel.Value.LayerID;
            transform.position = _basePosition.Value;
            tag = GeneralConstants.ItemTag;

            if (_targetPositionExist.Value)
            {
                await transform.DOMove(_targetPosition.Value, _mapConfig.ItemMapActivationDelay).SetEase(Ease.OutQuad).ToUniTask();
            }

            Collider.enabled = true;
        }
    }
}