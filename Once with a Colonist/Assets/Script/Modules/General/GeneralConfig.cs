using System.Collections.Generic;
using NaughtyAttributes;
using TendedTarsier.Script.Modules.Gameplay.Services.Map.MapItem;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General
{
    [CreateAssetMenu(menuName = "Config/GeneralConfig", fileName = "GeneralConfig")]
    public class GeneralConfig : ScriptableObject
    {
        [field: SerializeField, Scene]
        public string MenuScene { get; set; }

        [field: SerializeField, Scene]
        public string GameplayScene { get; set; }

        [field: SerializeField]
        public List<SimpleItem> MapItemsPreconditionList { get; set; } = new();
    }
}