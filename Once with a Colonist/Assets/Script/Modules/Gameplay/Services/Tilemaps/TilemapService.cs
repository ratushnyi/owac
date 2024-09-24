using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TendedTarsier.Script.Modules.Gameplay.Configs;
using TendedTarsier.Script.Modules.Gameplay.Configs.Tilemap;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.General.Services;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps
{
    [UsedImplicitly]
    public class TilemapService : ServiceBase
    {
        public IReadOnlyReactiveProperty<Tilemap> CurrentTilemap => _currentTilemap;
        
        private readonly ReactiveProperty<Tilemap> _currentTilemap = new();
        private readonly TilemapProfile _tilemapProfile;
        private readonly TilemapConfig _tilemapConfig;
        private readonly List<Tilemap> _tilemaps;

        private Vector3Int _lastTarget;

        private TilemapService(TilemapProfile tilemapProfile, TilemapConfig tilemapConfig, List<Tilemap> tilemaps)
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
                    tilemap.SetTile((Vector3Int)changedTile.Key, _tilemapConfig[changedTile.Value]);
                }
            }
        }

        public void OnGroundEnter(Tilemap groundTilemap)
        {
            _currentTilemap.Value = groundTilemap;
        }
        
        public void OnGroundExit(Tilemap groundTilemap)
        {
            if (_currentTilemap.Value == groundTilemap)
            {
                _currentTilemap.Value = null;
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
            _tilemapProfile.ChangedTiles[(Vector2Int)chords] = type;
            _tilemapProfile.Save();
        }

        public TileModel.TileType GetTile(Tilemap tilemap, Vector3Int chords)
        {
            var tile = tilemap.GetTile(chords);
            var model = _tilemapConfig.TilesModels.FirstOrDefault(t => t.Tile == tile);

            return model?.Type ?? TileModel.TileType.Default;
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
                if (tilemap.GetTile((Vector3Int)tilePosition) != null)
                {
                    return tilemap;
                }
            }

            return null;
        }
    }
}