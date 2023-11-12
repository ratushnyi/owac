using DG.Tweening;
using UnityEngine;

namespace TendedTarsier
{
    [CreateAssetMenu(menuName = "MenuConfig", fileName = "MenuConfig", order = 0)]
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