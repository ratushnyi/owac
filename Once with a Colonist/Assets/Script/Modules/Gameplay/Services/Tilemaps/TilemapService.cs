using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.General.Services;
using TendedTarsier.Script.Modules.General.Profiles.Tilemap;
using TendedTarsier.Script.Modules.General;
using TendedTarsier.Script.Modules.General.Configs;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps
{
    [UsedImplicitly]
    public class TilemapService : ServiceBase
    {
        public IReadOnlyReactiveProperty<Tilemap> CurrentTilemap => _currentTilemap;

        private readonly ReactiveProperty<Tilemap> _currentTilemap = new();
        private readonly MapProfile _mapProfile;
        private readonly MapConfig _mapConfig;
        private readonly List<Tilemap> _tilemaps;

        private Vector3Int _lastTarget;

        private TilemapService(
            [Inject(Id = GeneralConstants.GroundTilemapsListId)] 
            List<Tilemap> tilemaps,
            MapProfile mapProfile,
            MapConfig mapConfig)
        {
            _tilemaps = tilemaps;
            _mapConfig = mapConfig;
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
                    tilemap.SetTile((Vector3Int)changedTile.Key, _mapConfig[changedTile.Value]);
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
            var tile = _mapConfig[type];
            if (tile == null)
            {
                Debug.LogError($"{nameof(MapConfig)} does not contain a model for {type}");
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
            var model = _mapConfig.TileModelsList.FirstOrDefault(t => t.Tile == tile);

            return model?.Type ?? TileModel.TileType.None;
        }

        public void ProcessTarget(Vector3Int? targetPosition)
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
            return _tilemaps.FirstOrDefault(tilemap => tilemap.GetTile((Vector3Int)tilePosition) != null);
        }
    }
}