using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using UniRx;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Map;
using TendedTarsier.Script.Modules.General.Services.Input;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class EmitterHoldDeviceMapObject : DeviceMapObject
    {
        private readonly ISubject<Unit> _onPlayerExit = new Subject<Unit>();

        private MapService _mapService;
        private StatsService _statsService;
        private InputService _inputService;

        [SerializeField]
        private int _dropDistance = 2;
        [SerializeField]
        private Direction _direction;
        [SerializeField]
        private ItemEntity _emissionItem;
        [SerializeField]
        private StatFeeModel _statFeeModel;

        [Inject]
        private void Construct(MapService mapService, StatsService statsService, InputService inputService)
        {
            _mapService = mapService;
            _statsService = statsService;
            _inputService = inputService;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            switch (other.tag)
            {
                case GeneralConstants.PlayerTag:
                    _onPlayerExit.OnNext(Unit.Default);
                    break;
            }
        }

        public override async UniTask<bool> Perform()
        {
            if (!_statsService.IsSuitable(_statFeeModel.Type, _statFeeModel.Value))
            {
                return false;
            }

            if (_statFeeModel.Rate != 0)
            {
                var onPlayerExit = _onPlayerExit.First().ToUniTask();
                var onCancelPerformed = _inputService.OnXButtonCanceled.First().ToUniTask();
                var progressBar = _mapService.ShowProgressBar(this, _statFeeModel.Rate);

                var result = await UniTask.WhenAny(progressBar.awaiter, onCancelPerformed, onPlayerExit);

                if (result > 0)
                {
                    progressBar.disposable.Dispose();
                    return false;
                }
            }

            var targetPosition = transform.position + _direction.ToVector3() * _dropDistance;
            var mapItem = new ItemMapModel
            {
                ItemEntity = _emissionItem,
                SortingLayerID = SpriteRenderer.sortingLayerID,
                Position = targetPosition
            };

            if (!_statsService.ApplyValue(_statFeeModel.Type, _statFeeModel.Value))
            {
                return false;
            }

            _mapService.DropMapItem(mapItem, transform.position, targetPosition).Forget();
            return true;
        }
    }
}