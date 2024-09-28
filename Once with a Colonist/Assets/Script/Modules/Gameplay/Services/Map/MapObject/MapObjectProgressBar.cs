using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public class MapObjectProgressBar : MonoBehaviour, IDisposable
    {
        private Tween _tween;

        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private Canvas _canvas;

        public async UniTask ShowProgressBar(float duration)
        {
            _canvas.worldCamera = Camera.main;
            _slider.value = 0;
            _tween = _slider.DOValue(1, duration).SetEase(Ease.Linear).OnComplete(Dispose);
            await _tween.AwaitForComplete();
        }

        public void Dispose()
        {
            _tween.Kill();
            _tween = null;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _tween.Kill();
        }
    }
}