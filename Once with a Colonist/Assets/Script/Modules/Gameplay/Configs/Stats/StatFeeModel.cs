using System;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Stats
{
    [Serializable]
    public class StatFeeModel
    {
        [field:SerializeField] 
        public StatType Type {get;set;}
        [field:SerializeField] 
        public int Rate {get;set;}
        [field:SerializeField] 
        public int Value {get;set;}
    }
}