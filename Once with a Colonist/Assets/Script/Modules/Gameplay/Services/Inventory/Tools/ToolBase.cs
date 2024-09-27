using TendedTarsier.Script.Modules.Gameplay.Services.Stats;
using TendedTarsier.Script.Modules.General.Configs.Stats;
using UnityEngine;
using Zenject;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Inventory.Tools
{
    [CreateAssetMenu(menuName = "Items/ToolBase", fileName = "ToolBase")]
    public class ToolBase : ScriptableObject, IPerformable
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

        public virtual bool Perform()
        {
            return UseResources();
        }
    }
}