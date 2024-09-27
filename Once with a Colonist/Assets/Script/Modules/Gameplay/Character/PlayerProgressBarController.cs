using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    public class PlayerProgressBarController : MonoBehaviour
    {
        private Tween _tween;

        [SerializeField]
        private Slider _slider;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowProgressBar(float duration)
        {
            _slider.value = 0;
            gameObject.SetActive(true);
            _tween = _slider.DOValue(1, duration).SetEase(Ease.Linear).OnComplete(Cancel);
        }

        public void Cancel()
        {
            gameObject.SetActive(false);
            _tween.Kill();
            _tween = null;
        }
    }
}