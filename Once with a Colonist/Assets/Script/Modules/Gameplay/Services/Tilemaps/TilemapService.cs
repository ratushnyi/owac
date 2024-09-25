using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.Gameplay.Configs.Tilemap;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps
{
    [UsedImplicitly]
    public class TilemapService : ServiceBase
    {
        public IReadOnlyReactiveProperty<Tilemap> CurrentTilemap => _currentTilemap;

        private readonly ReactiveProperty<Tilemap> _currentTilemap = new();
        private readonly MapProfile _mapProfile;
        private readonly TilemapConfig _tilemapConfig;
        private readonly List<Tilemap> _tilemaps;

        private Vector3Int _lastTarget;

        private TilemapService(
            [Inject(Id = GameplayInstaller.GroundTilemapsListId)]
            List<Tilemap> tilemaps,
            MapProfile mapProfile, 
            TilemapConfig tilemapConfig)
        {
            _tilemaps = tilemaps;
            _tilemapConfig = tilemapConfig;
            _mapProfile = mapProfile;
        }

        protected override void Initialize()
        {
            base.Initialize();

            LoadTileMap();
        }

        private void LoadTileMap()
        {
            foreach (var changedTile in _mapProfile.ChangedTiles)
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

        public void ChangedTile(Vector3Int chords, TileModel.TileType type)
        {
            var tile = _tilemapConfig[type];
            if (tile == null)
            {
                Debug.LogError($"{nameof(TilemapConfig)} does not contain a model for {type}");
                return;
            }

            _currentTilemap.Value.SetTile(chords, tile);
            _mapProfile.ChangedTiles[(Vector2Int)chords] = type;
            _mapProfile.Save();
        }

        public TileModel.TileType GetTile(Vector3Int chords)
        {
            if (_currentTilemap.Value == null)
            {
                return TileModel.TileType.None;
            }
            
            var tile = _currentTilemap.Value.GetTile(chords);
            var model = _tilemapConfig.TileModelsList.FirstOrDefault(t => t.Tile == tile);

            return model?.Type ?? TileModel.TileType.None;
        }

        public void ProcessTiles(Vector3Int? targetPosition)
        {
            if (_currentTilemap.Value != null && targetPosition != null)
            {
                var currentPosition = _currentTilemap.Value.WorldToCell(targetPosition.Value);

                if (currentPosition != _lastTarget)
                {
                    _currentTilemap.Value.SetColor(_lastTarget, Color.white);

                    var tile = _currentTilemap.Value.GetTile(currentPosition);
                    if (tile != null)
                    {
                        _currentTilemap.Value.SetColor(currentPosition, Color.red);
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