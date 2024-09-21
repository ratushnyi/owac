using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Inventory.Items
{
    [CreateAssetMenu(menuName = "Items/PerformTest", fileName = "PerformTest")]
    public class PerformTest : PerformEntityBase
    {
        private TilemapService _tilemapService;

        [Inject]
        public void Construct(TilemapService tilemapService)
        {
            _tilemapService = tilemapService;
        }

        public override bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            if (tilemap == null)
            {
                return false;
            }

            if (_tilemapService.GetTile(tilemap, targetPosition) == TileModel.TileType.Stone)
            {
                Debug.Log($"{nameof(Tilemap)} already has {TileModel.TileType.Stone} at {targetPosition}");
                return false;
            }

            _tilemapService.ChangedTile(tilemap, targetPosition, TileModel.TileType.Stone);

            return true;
        }
    }
}