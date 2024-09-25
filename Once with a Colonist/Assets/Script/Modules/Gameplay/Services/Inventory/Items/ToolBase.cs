using UnityEngine;
using Zenject;
using TendedTarsier.Script.Modules.Gameplay.Configs.Stats;
using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.General.Profiles.Stats;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Items
{
    [CreateAssetMenu(menuName = "Items/ToolBase", fileName = "ToolBase")]
    public class ToolBase : ScriptableObject
    {
        [SerializeField]
        private StatType _statType;
        [SerializeField]
        private int _value;

        protected StatsService StatsService;

        protected bool UseResources() => StatsService.ApplyValue(_statType, _value);

        [Inject]
        public void Construct(StatsService statsService)
        {
            StatsService = statsService;
        }

        public virtual bool Perform(Vector3Int targetPosition)
        {
            return UseResources();
        }
    }
}