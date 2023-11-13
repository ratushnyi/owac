using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace TendedTarsier
{
    public class TilemapService
    {
        private TilemapProfile _tilemapProfile;
        private TilemapConfig _tilemapConfig;
        private List<Tilemap> _tilemaps;
        
        private Vector3Int _lastTarget;


        [Inject]
        private void Construct(TilemapProfile tilemapProfile, TilemapConfig tilemapConfig, List<Tilemap> tilemaps)
        {
            _tilemaps = tilemaps;
            _tilemapConfig = tilemapConfig;
            _tilemapProfile = tilemapProfile;

            LoadTileMap();
        }

        private void LoadTileMap()
        {
            foreach (var changedTile in _tilemapProfile.ChangedTiles)
            {
                var tilemap = GetTilemap(changedTile.Key);
                if (tilemap != null)
                {
                    tilemap.SetTile((Vector3Int) changedTile.Key, _tilemapConfig[changedTile.Value]);
                }
            }
        }

        public void ChangedTile(Tilemap tilemap, Vector3Int chords, TileModel.TileType type)
        {
            var tile = _tilemapConfig[type];
            if (tile == null)
            {
                Debug.LogError($"{nameof(TilemapConfig)} does not contain a model for {type}");
                return;
            }
            
            tilemap.SetTile(chords, tile);
            _tilemapProfile.ChangedTiles[(Vector2Int) chords] = type;
            _tilemapProfile.Save();
        }
        
        
        public void ProcessTiles(Tilemap tilemap, Vector3Int? targetPosition)
        {
            if (tilemap != null && targetPosition != null)
            {
                var currentPosition = tilemap.WorldToCell(targetPosition.Value);

                if (currentPosition != _lastTarget)
                {
                    tilemap.SetColor(_lastTarget, Color.white);

                    var tile = tilemap.GetTile(currentPosition);
                    if (tile != null)
                    {
                        tilemap.SetColor(currentPosition, Color.red);
                    }
                    _lastTarget = currentPosition;
                }
            }
        }

        public Tilemap GetTilemap(Vector2Int tilePosition)
        {
            foreach (var tilemap in _tilemaps)
            {
                if (tilemap.GetTile((Vector3Int) tilePosition) != null)
                {
                    return tilemap;
                }
            }

            return null;
        }
    }
}