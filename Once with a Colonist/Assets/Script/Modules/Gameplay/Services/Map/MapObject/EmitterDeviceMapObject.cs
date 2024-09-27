using System;
using Cysharp.Threading.Tasks;
using TendedTarsier.Script.Modules.Gameplay.Character;
using UnityEngine;
using Zenject;
using TendedTarsier.Script.Utilities.Extensions;
using TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Map;
using TendedTarsier.Script.Modules.General.Services.Input;
using UniRx;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class EmitterDeviceMapObject : DeviceMapObject
    {
        private ISubject<Unit> _onPlayerExit = new Subject<Unit>();
        private MapService _mapService;
        private StatsService _statsService;
        private InputService _inputService;
        private PlayerProgressBarController _playerProgressBarController;

        [SerializeField]
        public int _dropDistance = 2;
        [SerializeField]
        public Direction _direction;
        [SerializeField]
        public ItemEntity _emissionItem;
        [SerializeField]
        public StatFeeModel _statFeeModel;

        [Inject]
        private void Construct(MapService mapService, StatsService statsService, InputService inputService, PlayerProgressBarController playerProgressBarController)
        {
            _mapService = mapService;
            _statsService = statsService;
            _inputService = inputService;
            _playerProgressBarController = playerProgressBarController;
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
                var onPerformSuccess = UniTask.Delay(TimeSpan.FromSeconds(_statFeeModel.Rate));
                var onPlayerExit = _onPlayerExit.First().ToUniTask();
                var onCancelPerformed = _inputService.OnXButtonCanceled.First().ToUniTask();
                _playerProgressBarController.ShowProgressBar(_statFeeModel.Rate);
                var result = await UniTask.WhenAny(onPerformSuccess, onCancelPerformed, onPlayerExit);

                if (result > 0)
                {
                    _playerProgressBarController.Cancel();
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