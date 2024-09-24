using System;
using System.Collections.Generic;

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

        public StatType StatType;
        public List<StatLevelModel> Levels;
    }
}