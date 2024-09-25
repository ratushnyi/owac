using System.Collections.Generic;
using System.Linq;
using TendedTarsier.Script.Modules.Gameplay.Panels.HUD;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Stats
{
    [CreateAssetMenu(menuName = "Config/StatsConfig", fileName = "StatsConfig")]
    public class StatsConfig : ScriptableObject
    {
        [field: SerializeField]
        public StatBarView StatBarView { get; set; }

        [field: SerializeField]
        public List<StatModel> StatsList { get; set; }

        [field: SerializeField]
        public List<StatFeeConditionalModel> StatsFeeConditionalList { get; set; }

        [field: SerializeField]
        public StatFeeModel RunFee { get; set; }

        [field: SerializeField]
        public int WalkSpeed { get; set; } = 1;

        [field: SerializeField]
        public int RunSpeed { get; set; } = 2;

        public StatModel GetStatsModel(StatType statType)
        {
            return StatsList.FirstOrDefault(t => t.StatType == statType);
        }
    }
}