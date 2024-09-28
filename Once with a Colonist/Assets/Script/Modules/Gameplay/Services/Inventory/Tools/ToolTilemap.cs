using Cysharp.Threading.Tasks;
using TendedTarsier.Script.Modules.Gameplay.Services.Player;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Tools
{
    [CreateAssetMenu(menuName = "Items/ToolTilemap", fileName = "ToolTilemap")]
    public class ToolTilemap : ToolBase
    {
        private TilemapService _tilemapService;
        private PlayerService _playerService;

        [SerializeField]
        private TileModel.TileType _tileType;

        [Inject]
        public void Construct(TilemapService tilemapService, PlayerService playerService)
        {
            _tilemapService = tilemapService;
            _playerService = playerService;
        }

        public override UniTask<bool> Perform()
        {
            if (_tilemapService.CurrentTilemap.Value == null)
            {
                return new UniTask<bool>(false);
            }

            var targetPosition = _playerService.TargetPosition;
            if (_tilemapService.GetTile(targetPosition) == _tileType)
            {
                return new UniTask<bool>(false);
            }

            if (!UseResources())
            {
                return new UniTask<bool>(false);
            }

            _tilemapService.ChangedTile(targetPosition, _tileType);
            return new UniTask<bool>(true);
        }
    }
}