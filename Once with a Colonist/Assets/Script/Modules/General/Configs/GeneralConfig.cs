using NaughtyAttributes;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Configs
{
    [CreateAssetMenu(menuName = "Config/GeneralConfig", fileName = "GeneralConfig")]
    public class GeneralConfig : ScriptableObject
    {
        [field: SerializeField, Scene]
        public string MenuScene { get; set; }

        [field: SerializeField, Scene]
        public string GameplayScene { get; set; }
    }
}