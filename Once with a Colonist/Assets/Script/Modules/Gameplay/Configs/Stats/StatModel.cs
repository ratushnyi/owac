using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Stats
{
    [Serializable]
    public class StatModel
    {
        [Serializable]
        public class StatLevelModel
        {
            public int BorderValue;
            public int Range;
            public int RecoveryRate;
        }

        [SerializeField]
        private List<StatLevelModel> Levels;
        public StatType StatType;

        public StatLevelModel GetLevel(int level)
        {
            return Levels.Count > level ? Levels[level] : Levels.Last();
        }
    }
}