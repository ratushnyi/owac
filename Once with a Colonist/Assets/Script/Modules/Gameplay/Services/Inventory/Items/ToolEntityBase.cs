using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using UnityEngine.Serialization;
using StatsProfile = TendedTarsier.Script.Modules.General.Profiles.Stats.StatsProfile;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    public class ToolEntityBase : ScriptableObject
    {
        [SerializeField]
        private StatType _statType;
        [SerializeField]
        private int _value;

        protected StatsProfile StatsProfile;
        protected StatsService StatsService;

        protected bool IsEnoughResources => StatsProfile.StatsDictionary[_statType].Value.Value >= _value;
        protected void UseResources() => StatsService.ApplyValue(_statType, _value);

        [Inject]
        public void Construct(StatsProfile statsProfile, StatsService statsService)
        {
            StatsProfile = statsProfile;
            StatsService = statsService;
        }

        public virtual bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            return false;
        }
    }
}