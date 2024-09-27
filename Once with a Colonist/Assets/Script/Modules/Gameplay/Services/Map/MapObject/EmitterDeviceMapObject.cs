using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using TendedTarsier.Script.Modules.General.Profiles.Map;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class EmitterDeviceMapObject : DeviceMapObject
    {
        private MapService _mapService;

        [SerializeField]
        public int _dropDistance = 2;
        [SerializeField]
        public Direction _direction;
        [SerializeField]
        public ItemEntity _emissionItem;

        [Inject]
        private void Construct(MapService mapService)
        {
            _mapService = mapService;
        }

        public override bool Perform()
        {
            var targetPosition = transform.position + _direction.ToVector3() * _dropDistance;
            var mapItem = new ItemMapModel
            {
                ItemEntity = _emissionItem,
                SortingLayerID = SpriteRenderer.sortingLayerID,
                Position = targetPosition
            };
            _mapService.DropMapItem(mapItem, transform.position, targetPosition).Forget();
            return true;
        }
    }
}