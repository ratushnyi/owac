using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Character;
using TendedTarsier.Script.Modules.Gameplay.Configs;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    public class PerformEntityBase : ScriptableObject
    {
        [SerializeField]
        private StatType _statType;
        [SerializeField]
        private int _requiredValue;

        protected StatsProfile StatsProfile;

        protected bool IsEnoughResources => StatsProfile.StatsDictionary[_statType].Value >= _requiredValue;
        protected void UseResources() => StatsProfile.StatsDictionary[_statType].Value -= _requiredValue;

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