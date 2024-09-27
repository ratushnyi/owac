using Cysharp.Threading.Tasks;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Tools
{
    [CreateAssetMenu(menuName = "Items/ToolTilemap", fileName = "ToolTilemap")]
    public class ToolTilemap : ToolBase
    {
        private TilemapService _tilemapService;
        private PlayerController _playerController;

        [SerializeField]
        private TileModel.TileType _tileType;

        [Inject]
        public void Construct(TilemapService tilemapService, PlayerController playerController)
        {
            _tilemapService = tilemapService;
            _playerController = playerController;
        }

        public override UniTask<bool> Perform()
        {
            if (_tilemapService.CurrentTilemap.Value == null)
            {
                return new UniTask<bool>(false);
            }

            if (_tilemapService.GetTile(_playerController.TargetPosition.Value) == _tileType)
            {
                return new UniTask<bool>(false);
            }

            if (!UseResources())
            {
                return new UniTask<bool>(false);
            }

            _tilemapService.ChangedTile(_playerController.TargetPosition.Value, _tileType);
            return new UniTask<bool>(true);
        }
    }
}