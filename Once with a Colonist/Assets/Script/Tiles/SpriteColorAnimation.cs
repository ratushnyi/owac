using UnityEngine;

namespace TendedTarsier.Tiles
{
    public class SpriteColorAnimation : MonoBehaviour
    {
        [SerializeField]
        private Gradient _gradient;

        [SerializeField]
        private float _time;

        private SpriteRenderer _spriteRenderer;
        private float _timer;

        private void Start()
        {
            _timer = _time * Random.value;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_spriteRenderer)
            {
                _timer += Time.deltaTime;
                if (_timer > _time) _timer = 0.0f;

                _spriteRenderer.color = _gradient.Evaluate(_timer / _time);
            }
        }
    }
}