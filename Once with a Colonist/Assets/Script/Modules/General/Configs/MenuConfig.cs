using DG.Tweening;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Configs
{
    [CreateAssetMenu(menuName = "Config/MenuConfig", fileName = "MenuConfig")]
    public class MenuConfig : ScriptableObject
    {
        [field: SerializeField]
        public float BackgroundFadeOutDuration { get; set; }

        [field: SerializeField]
        public Color BackgroundFadeOutColor { get; set; }

        [field: SerializeField]
        public Ease BackgroundFadeOutCurve { get; set; }
    }
}