using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class ItemMapObject : MapObjectBase
    {
        private ItemModel _itemModel;

        [field: SerializeField]
        public ItemEntity ItemEntity { get; set; }

        private void Start()
        {
            tag = GeneralConstants.ItemTag;
            Collider.enabled = false;
        }

        public void Setup(ItemModel itemModel, MapItemModel mapItemModel, Vector3 position)
        {
            _itemModel = itemModel;
            ItemEntity = mapItemModel.ItemEntity;
            SpriteRenderer.sprite = _itemModel.Sprite;
            SpriteRenderer.sortingLayerID = mapItemModel.SortingLayerID;
            transform.position = position;
        }

        public async UniTask DoMove(Vector3 targetPosition)
        {
            await transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutQuad).ToUniTask();
        }

        public void Init(int layer, int sortingLayerID, float activationDelay)
        {
            SpriteRenderer.sortingLayerID = sortingLayerID;
            gameObject.layer = layer;
            Observable.Timer(TimeSpan.FromSeconds(activationDelay)).First().Subscribe(_ => { Collider.enabled = true; });
        }
    }
}