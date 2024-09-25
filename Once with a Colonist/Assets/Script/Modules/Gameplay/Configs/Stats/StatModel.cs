using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Stats
{
    [Serializable]
    public class StatModel
    {
        [FormerlySerializedAs("TabBar")]
        public bool StatBar;
        public bool Observable;

        [SerializeField]
        private List<StatLevelModel> Levels;
        public StatType StatType;

        public StatLevelModel GetLevel(int level)
        {
            return Levels.Count > level ? Levels[level] : Levels.Last();
        }
    }
}