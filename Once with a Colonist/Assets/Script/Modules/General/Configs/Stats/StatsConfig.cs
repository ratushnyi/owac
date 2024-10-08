using System.Collections.Generic;
using System.Linq;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Configs.Stats
{
    [CreateAssetMenu(menuName = "Config/StatsConfig", fileName = "StatsConfig")]
    public class StatsConfig : ScriptableObject
    {
        public StatModel this[StatType type] => StatsList.FirstOrDefault(t => t.StatType == type);

        [field: SerializeField]
        public StatBarView StatBarView { get; set; }

        [field: SerializeField]
        public List<StatModel> StatsList { get; set; }

        [field: SerializeField]
        public List<StatFeeConditionModel> StatsFeeConditionalList { get; set; }
    }
}