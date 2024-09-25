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
        public StatType StatType;
        public Sprite Sprite;
        public bool StatBar;
        public bool Observable;

        [SerializeField]
        private List<StatLevelModel> Levels;

        public StatLevelModel GetLevel(int level)
        {
            return Levels.Count > level ? Levels[level] : Levels.Last();
        }
    }
}