using System;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Stats
{
    [Serializable]
    public class StatFeeModel
    {
        public StatType Type;
        public int Rate;
        public int Value;
    }

    [Serializable]
    public class StatFeeConditionalModel
    {
        public enum FeeConditionalType
        {
            MinValue = 0,
            MaxValue = 1,
        }

        public StatType Type;
        public FeeConditionalType Rate;
        public StatFeeModel FeeModel;
    }
}