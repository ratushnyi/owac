using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using StatsProfile = TendedTarsier.Script.Modules.General.Profiles.Stats.StatsProfile;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    public class ToolEntityBase : ScriptableObject
    {
        [SerializeField]
        private StatType _statType;
        [SerializeField]
        private int _requiredValue;

        protected StatsProfile StatsProfile;

        protected bool IsEnoughResources => StatsProfile.StatsDictionary[_statType].CurrentValue.Value >= _requiredValue;
        protected void UseResources() => StatsProfile.StatsDictionary[_statType].CurrentValue.Value -= _requiredValue;

        [Inject]
        public void Construct(StatsProfile statsProfile)
        {
            StatsProfile = statsProfile;
        }

        public virtual bool Perform(Tilemap tilemap, Vector3Int targetPosition)
        {
            return false;
        }
    }
}