using System;
using NaughtyAttributes;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Stats
{
    [Serializable]
    public class StatLevelModel
    {
        public int BorderValue;
        public int Range;
        public int DefaultValue;
        public int RecoveryRate;
        public int RecoveryValue;
    }
}