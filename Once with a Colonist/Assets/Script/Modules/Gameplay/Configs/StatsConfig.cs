using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Configs
{
    [CreateAssetMenu(menuName = "Config/StatsConfig", fileName = "StatsConfig")]
    public class StatsConfig : ScriptableObject
    {
        [field: SerializeField]
        public List<StatEntity> StatsList { get; set; }

        public StatEntity GetStatsEntity(StatType statType)
        {
            return StatsList.FirstOrDefault(t => t.StatType == statType);
        }
    }
    
    [Serializable]
    public enum StatType
    {
        EnergyLevel,
        Energy,
        Health,
        Strange
    }

    [Serializable]
    public class StatEntity
    {
        [Serializable]
        public class StatLevelEntity
        {
            public int BorderValue;
            public int Range;
            public int RecoveryRate;
        }

        public StatType StatType;
        public List<StatLevelEntity> Levels;
    }
}