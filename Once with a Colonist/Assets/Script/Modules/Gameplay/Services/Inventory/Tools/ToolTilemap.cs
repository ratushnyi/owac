using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Tools
{
    [CreateAssetMenu(menuName = "Items/ToolTilemap", fileName = "ToolTilemap")]
    public class ToolTilemap : ToolBase
    {
        [SerializeField]
        private TileModel.TileType _tileType;
        private TilemapService _tilemapService;

        [Inject]
        public void Construct(TilemapService tilemapService)
        {
            _tilemapService = tilemapService;
        }

        public override bool Perform(Vector3Int targetPosition)
        {
            if (_tilemapService.CurrentTilemap.Value == null)
            {
                return false;
            }

            if (_tilemapService.GetTile(targetPosition) == _tileType)
            {
                return false;
            }

            if (!UseResources())
            {
                return false;
            }

            _tilemapService.ChangedTile(targetPosition, _tileType);
            return true;
        }
    }
}