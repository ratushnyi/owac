using Cysharp.Threading.Tasks;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using TendedTarsier.Script.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class EmitterDelayDeviceMapObject : DeviceMapObject
    {
        private AsyncLazy _awaiter;
        private MapService _mapService;
        private StatsService _statsService;

        [SerializeField]
        private int _dropDistance = 2;
        [SerializeField]
        private Direction _direction;
        [SerializeField]
        private ItemEntity _emissionItem;
        [SerializeField]
        private StatFeeModel _statFeeModel;

        [Inject]
        private void Construct(MapService mapService, StatsService statsService)
        {
            _mapService = mapService;
            _statsService = statsService;
        }

        public override async UniTask<bool> Perform()
        {
            if (_awaiter is { Task: { Status: UniTaskStatus.Pending } } || !_statsService.ApplyValue(_statFeeModel.Type, _statFeeModel.Value))
            {
                return false;
            }

            if (_statFeeModel.Rate != 0)
            {
                _awaiter = _mapService.ShowProgressBar(this, _statFeeModel.Rate).awaiter.ToAsyncLazy();
                await _awaiter;
            }

            var targetPosition = transform.position + _direction.ToVector3() * _dropDistance;
            _mapService.DropMapItem(_emissionItem, transform.position, targetPosition);
            return true;
        }
    }
}