using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    [CreateAssetMenu(menuName = "Items/ToolTest", fileName = "ToolTest")]
    public class ToolTest : ToolEntityBase
    {
        private TilemapService _tilemapService;

        [Inject]
        public void Construct(TilemapService tilemapService)
        {
            _tilemapService = tilemapService;
        }

        public override bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            if (!IsEnoughResources)
            {
                return false;
            }

            if (tilemap == null)
            {
                return false;
            }

            if (_tilemapService.GetTile(tilemap, targetPosition) == TileModel.TileType.Stone)
            {
                return false;
            }

            _tilemapService.ChangedTile(tilemap, targetPosition, TileModel.TileType.Stone);

            UseResources();
            return true;
        }
    }
}