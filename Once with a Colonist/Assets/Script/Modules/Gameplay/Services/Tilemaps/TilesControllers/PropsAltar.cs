using System.Collections.Generic;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps.TilesControllers
{
    public class PropsAltar : MonoBehaviour
    {
        [SerializeField]
        private List<SpriteRenderer> _runes;

        [SerializeField]
        private float _lerpSpeed;

        private Color _curColor;
        private Color _targetColor;

        private void OnTriggerEnter2D(Collider2D other)
        {
            _targetColor = new Color(1, 1, 1, 1);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _targetColor = new Color(1, 1, 1, 0);
        }

        private void Update()
        {
            _curColor = Color.Lerp(_curColor, _targetColor, _lerpSpeed * Time.deltaTime);

            foreach (var r in _runes)
            {
                r.color = _curColor;
            }
        }
    }
}